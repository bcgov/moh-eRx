/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Message
    {
        private const string SegmentRegex = "^[A-Z][A-Z][A-Z1-9]$";
        private const string FieldRegex = @"^([0-9]+)([\(\[]([0-9]+)[\)\]]){0,1}$";
        private const string OtherRegEx = @"^[1-9]([0-9]{1,2})?$";
        private IList<string> allSegments = new List<string>();

        public Message()
        {
        }

        public Message(string strMessage)
        {
            this.HL7Message = strMessage;
        }

        public string HL7Message { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string MessageStructure { get; set; } = string.Empty;

        public string MessageControlID { get; set; } = string.Empty;

        public string ProcessingID { get; set; } = string.Empty;

        public short SegmentCount { get; set; }

        public HL7Encoding Encoding { get; set; } = new HL7Encoding();

        internal Dictionary<string, List<Segment>> SegmentList { get; set; } = new Dictionary<string, List<Segment>>();

        public override int GetHashCode()
        {
            return this.HL7Message.GetHashCode(StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Parse the HL7 message in text format, throws HL7Exception if error occurs
        /// </summary>
        /// <returns>boolean</returns>
        public bool ParseMessage()
        {
            bool isValid = false;
            bool isParsed = false;

            isValid = this.ValidateMessage(); // Throws exceptions.

            if (isValid)
            {
                try
                {
                    if (this.allSegments == null || this.allSegments.Count <= 0)
                    {
                        this.allSegments = MessageHelper.SplitMessage(this.HL7Message);
                    }

                    short segSeqNo = 0;

                    foreach (string strSegment in this.allSegments)
                    {
                        if (string.IsNullOrWhiteSpace(strSegment))
                        {
                            continue;
                        }

                        Segment newSegment = new Segment(this.Encoding)
                        {
                            Name = strSegment.Substring(0, 3),
                            Value = strSegment,
                            SequenceNo = segSeqNo++,
                        };

                        this.AddNewSegment(newSegment);
                    }

                    this.SegmentCount = segSeqNo;

                    string strSerializedMessage = string.Empty;

                    try
                    {
                        strSerializedMessage = this.SerializeMessage(false);
                    }
                    catch (HL7Exception ex)
                    {
                        throw new HL7Exception("Failed to serialize parsed message with error - " + ex.Message, HL7Exception.PARSING_ERROR);
                    }

                    if (!string.IsNullOrEmpty(strSerializedMessage))
                    {
                        if (this.Equals(strSerializedMessage))
                        {
                            isParsed = true;
                        }
                    }
                    else
                    {
                        throw new HL7Exception("Unable to serialize to original message - ", HL7Exception.PARSING_ERROR);
                    }
                }
                catch (Exception ex)
                {
                    throw new HL7Exception("Failed to parse the message with error - " + ex.Message, HL7Exception.PARSING_ERROR);
                }
            }

            return isParsed;
        }

        /// <summary>
        /// Serialize the message in text format
        /// </summary>
        /// <param name="validate">Validate the message before serializing</param>
        /// <returns>string with HL7 message</returns>
        public string SerializeMessage(bool validate)
        {
            if (validate && !this.ValidateMessage())
            {
                throw new HL7Exception("Failed to validate the updated message", HL7Exception.BAD_MESSAGE);
            }

            var strMessage = new StringBuilder();
            string currentSegName = string.Empty;
            List<Segment> segListOrdered = this.GetAllSegmentsInOrder();

            try
            {
                try
                {
                    foreach (Segment seg in segListOrdered)
                    {
                        currentSegName = seg.Name;

                        strMessage.Append(seg.Name);

                        if (seg.FieldList.Count > 0)
                        {
                            strMessage.Append(this.Encoding.FieldDelimiter);
                        }

                        int startField = currentSegName == "MSH" ? 1 : 0;

                        for (int i = startField; i < seg.FieldList.Count; i++)
                        {
                            if (i > startField)
                            {
                                strMessage.Append(this.Encoding.FieldDelimiter);
                            }

                            var field = seg.FieldList[i];

                            if (field.IsDelimiters)
                            {
                                strMessage.Append(field.Value);
                                continue;
                            }

                            if (field.HasRepetitions)
                            {
                                for (int j = 0; j < field.RepetitionList.Count; j++)
                                {
                                    if (j > 0)
                                    {
                                        strMessage.Append(this.Encoding.RepeatDelimiter);
                                    }

                                    this.SerializeField(field.RepetitionList[j], strMessage);
                                }
                            }
                            else
                            {
                                this.SerializeField(field, strMessage);
                            }
                        }

                        strMessage.Append(this.Encoding.SegmentDelimiter);
                    }
                }
                catch (Exception ex)
                {
                    if (currentSegName == "MSH")
                    {
                        throw new HL7Exception("Failed to serialize the MSH segment with error - " + ex.Message, HL7Exception.SERIALIZATION_ERROR);
                    }
                    else
                    {
                        throw;
                    }
                }

                return strMessage.ToString();
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Failed to serialize the message with error - " + ex.Message, HL7Exception.SERIALIZATION_ERROR);
            }
        }

        /// <summary>
        /// Get the Value of specific Field/Component/SubCpomponent, throws error if field/component index is not valid
        /// </summary>
        /// <param name="strValueFormat">Field/Component position in format SEGMENTNAME.FieldIndex.ComponentIndex.SubComponentIndex example PID.5.2</param>
        /// <returns>Value of specified field/component/subcomponent</returns>
        public string GetValue(string strValueFormat)
        {
            string segmentName = string.Empty;
            int componentIndex = 0;
            int subComponentIndex = 0;
            string strValue = string.Empty;
            IList<string> allComponents = MessageHelper.SplitString(strValueFormat, new char[] { '.' });

            int comCount = allComponents.Count;
            bool isValid = ValidateValueFormat(allComponents);

            if (isValid)
            {
                segmentName = allComponents[0];

                if (this.SegmentList.ContainsKey(segmentName))
                {
                    var segment = this.SegmentList[segmentName].First();

                    if (comCount == 4)
                    {
                        try
                        {
                            componentIndex = int.Parse(allComponents[2], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
                            subComponentIndex = int.Parse(allComponents[3], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

                            var field = GetField(segment, allComponents[1]);
                            strValue = field.ComponentList[componentIndex - 1].SubComponentList[subComponentIndex - 1].Value;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("SubComponent not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                    else if (comCount == 3)
                    {
                        try
                        {
                            componentIndex = int.Parse(allComponents[2], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
                            var field = GetField(segment, allComponents[1]);
                            strValue = field.ComponentList[componentIndex - 1].Value;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("Component not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                    else if (comCount == 2)
                    {
                        try
                        {
                            var field = GetField(segment, allComponents[1]);
                            strValue = field.Value;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("Field not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            strValue = segment.Value;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("Segment value not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                }
                else
                {
                    throw new HL7Exception("Segment name not available: " + strValueFormat);
                }
            }
            else
            {
                throw new HL7Exception("Request format is not valid: " + strValueFormat);
            }

            return strValue;
        }

        /// <summary>
        /// Sets the Value of specific Field/Component/SubComponent, throws error if field/component index is not valid
        /// </summary>
        /// <param name="strValueFormat">Field/Component position in format SEGMENTNAME.FieldIndex.ComponentIndex.SubComponentIndex example PID.5.2</param>
        /// <param name="strValue">Value for the specified field/component</param>
        /// <returns>boolean</returns>
        public bool SetValue(string strValueFormat, string strValue)
        {
            bool isSet = false;
            string segmentName = string.Empty;
            int componentIndex = 0;
            int subComponentIndex = 0;
            IList<string> allComponents = MessageHelper.SplitString(strValueFormat, new char[] { '.' });
            int comCount = allComponents.Count;
            bool isValid = ValidateValueFormat(allComponents);

            if (isValid)
            {
                segmentName = allComponents[0];

                if (this.SegmentList.ContainsKey(segmentName))
                {
                    var segment = this.SegmentList[segmentName].First();

                    if (comCount == 4)
                    {
                        try
                        {
                            componentIndex = int.Parse(allComponents[2], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
                            subComponentIndex = int.Parse(allComponents[3], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
                            var field = GetField(segment, allComponents[1]);
                            field.ComponentList[componentIndex - 1].SubComponentList[subComponentIndex - 1].Value = strValue;
                            isSet = true;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("SubComponent not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                    else if (comCount == 3)
                    {
                        try
                        {
                            componentIndex = int.Parse(allComponents[2], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);

                            var field = GetField(segment, allComponents[1]);
                            field.ComponentList[componentIndex - 1].Value = strValue;
                            isSet = true;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("Component not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                    else if (comCount == 2)
                    {
                        try
                        {
                            var field = GetField(segment, allComponents[1]);
                            field.Value = strValue;
                            isSet = true;
                        }
                        catch (Exception ex)
                        {
                            throw new HL7Exception("Field not available - " + strValueFormat + " Error: " + ex.Message);
                        }
                    }
                    else
                    {
                        throw new HL7Exception("Cannot overwrite a segment value");
                    }
                }
                else
                {
                    throw new HL7Exception("Segment name not available");
                }
            }
            else
            {
                throw new HL7Exception("Request format is not valid");
            }

            return isSet;
        }

        /// <summary>
        /// check if specified field has components
        /// </summary>
        /// <param name="strValueFormat">Field/Component position in format SEGMENTNAME.FieldIndex.ComponentIndex.SubComponentIndex example PID.5.2</param>
        /// <returns>boolean</returns>
        public bool IsComponentized(string strValueFormat)
        {
            bool isComponentized = false;
            string segmentName = string.Empty;
            IList<string> allComponents = MessageHelper.SplitString(strValueFormat, new char[] { '.' });
            int comCount = allComponents.Count;
            bool isValid = ValidateValueFormat(allComponents);

            if (isValid)
            {
                segmentName = allComponents[0];

                if (comCount >= 2)
                {
                    try
                    {
                        var segment = this.SegmentList[segmentName].First();
                        var field = GetField(segment, allComponents[1]);

                        isComponentized = field.IsComponentized;
                    }
                    catch (Exception ex)
                    {
                        throw new HL7Exception("Field not available - " + strValueFormat + " Error: " + ex.Message);
                    }
                }
                else
                {
                    throw new HL7Exception("Field not identified in request");
                }
            }
            else
            {
                throw new HL7Exception("Request format is not valid");
            }

            return isComponentized;
        }

        /// <summary>
        /// check if specified fields has repetitions.
        /// </summary>
        /// <param name="strValueFormat">Field/Component position in format SEGMENTNAME.FieldIndex.ComponentIndex.SubComponentIndex example PID.5.2</param>
        /// <returns>boolean</returns>
        public bool HasRepetitions(string strValueFormat)
        {
            bool hasRepetitions = false;
            string segmentName = string.Empty;
            IList<string> allComponents = MessageHelper.SplitString(strValueFormat, new char[] { '.' });
            int comCount = allComponents.Count;
            bool isValid = ValidateValueFormat(allComponents);

            if (isValid)
            {
                segmentName = allComponents[0];

                if (comCount >= 2)
                {
                    try
                    {
                        var segment = this.SegmentList[segmentName].First();
                        var field = GetField(segment, allComponents[1]);

                        hasRepetitions = field.HasRepetitions;
                    }
                    catch (Exception ex)
                    {
                        throw new HL7Exception("Field not available - " + strValueFormat + " Error: " + ex.Message);
                    }
                }
                else
                {
                    throw new HL7Exception("Field not identified in request");
                }
            }
            else
            {
                throw new HL7Exception("Request format is not valid");
            }

            return hasRepetitions;
        }

        /// <summary>
        /// check if specified component has sub components
        /// </summary>
        /// <param name="strValueFormat">Field/Component position in format SEGMENTNAME.FieldIndex.ComponentIndex.SubComponentIndex example PID.5.2</param>
        /// <returns>boolean</returns>
        public bool IsSubComponentized(string strValueFormat)
        {
            bool isSubComponentized = false;
            string segmentName = string.Empty;
            int componentIndex = 0;
            IList<string> allComponents = MessageHelper.SplitString(strValueFormat, new char[] { '.' });
            int comCount = allComponents.Count;
            bool isValid = ValidateValueFormat(allComponents);

            if (isValid)
            {
                segmentName = allComponents[0];

                if (comCount >= 3)
                {
                    try
                    {
                        var segment = this.SegmentList[segmentName].First();
                        var field = GetField(segment, allComponents[1]);

                        componentIndex = int.Parse(allComponents[2], NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
                        isSubComponentized = field.ComponentList[componentIndex - 1].IsSubComponentized;
                    }
                    catch (Exception ex)
                    {
                        throw new HL7Exception("Component not available - " + strValueFormat + " Error: " + ex.Message);
                    }
                }
                else
                {
                    throw new HL7Exception("Component not identified in request");
                }
            }
            else
            {
                throw new HL7Exception("Request format is not valid");
            }

            return isSubComponentized;
        }

        /// <summary>
        /// Builds the acknowledgement message for this message
        /// </summary>
        /// <returns>An ACK message if success, otherwise null</returns>
        public Message? GetACK()
        {
            return this.CreateAckMessage("AA", false, null);
        }

        /// <summary>
        /// Builds a negative ack for this message
        /// </summary>
        /// <param name="code">ack code like AR, AE</param>
        /// <param name="errMsg">Error message to be sent with NACK</param>
        /// <returns>A NACK message if success, otherwise null</returns>
        public Message? GetNACK(string code, string errMsg)
        {
            return this.CreateAckMessage(code, true, errMsg);
        }

        /// <summary>
        /// Adds a segemnt to the message
        /// </summary>
        /// <param name="newSegment">Segment to be appended to the end of the message</param>
        /// <returns>True if added successfully, otherwise false</returns>
        public bool AddNewSegment(Segment newSegment)
        {
            try
            {
                newSegment.SequenceNo = this.SegmentCount++;

                if (!this.SegmentList.ContainsKey(newSegment.Name))
                {
                    this.SegmentList[newSegment.Name] = new List<Segment>();
                }

                this.SegmentList[newSegment.Name].Add(newSegment);
                return true;
            }
            catch (Exception ex)
            {
                this.SegmentCount--;
                throw new HL7Exception("Unable to add new segment. Error - " + ex.Message);
            }
        }

        /// <summary>
        /// Removes a segment from the message
        /// </summary>
        /// <param name="segmentName">Segment to be removed</param>
        /// <param name="index">Zero-based index of the segment to be removed, in case of multiple. Default is 0.</param>
        /// <returns>True if found and removed sucessfully, otherwise false</returns>
        public bool RemoveSegment(string segmentName, int index = 0)
        {
            try
            {
                if (!this.SegmentList.ContainsKey(segmentName))
                {
                    return false;
                }

                var list = this.SegmentList[segmentName];
                if (list.Count <= index)
                {
                    return false;
                }

                list.RemoveAt(index);
                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Unable to add remove segment. Error - " + ex.Message);
            }
        }

        public IList<Segment> Segments()
        {
            return this.GetAllSegmentsInOrder();
        }

        public IList<Segment> Segments(string segmentName)
        {
            return this.GetAllSegmentsInOrder().FindAll(o => o.Name.Equals(segmentName, StringComparison.Ordinal));
        }

        public Segment DefaultSegment(string segmentName)
        {
            return this.GetAllSegmentsInOrder().First(o => o.Name.Equals(segmentName, StringComparison.Ordinal));
        }

        /// <summary>
        /// Addsthe header segment to a new message
        /// </summary>
        /// <param name="sendingApplication">Sending application name</param>
        /// <param name="sendingFacility">Sending facility name</param>
        /// <param name="receivingApplication">Receiving application name</param>
        /// <param name="receivingFacility">Receiving facility name</param>
        /// <param name="security">Security features. Can be null.</param>
        /// <param name="messageType">Message type ^ trigger event</param>
        /// <param name="messageControlID">Message control unique ID</param>
        /// <param name="processingID">Processing ID ^ processing mode</param>
        /// <param name="version">HL7 message version (2.x)</param>
        public void AddSegmentMSH(
            string sendingApplication,
            string sendingFacility,
            string receivingApplication,
            string receivingFacility,
            string security,
            string messageType,
            string messageControlID,
            string processingID,
            string version)
        {
            var dateString = MessageHelper.LongDateWithFractionOfSecond(DateTime.Now);
            var delim = this.Encoding.FieldDelimiter;

            string response = "MSH" + this.Encoding.AllDelimiters + delim + sendingApplication + delim + sendingFacility + delim
                + receivingApplication + delim + receivingFacility + delim
                + dateString + delim + (security ?? string.Empty) + delim + messageType + delim + messageControlID + delim
                + processingID + delim + version + this.Encoding.SegmentDelimiter;

            var message = new Message(response);
            message.ParseMessage();
            this.AddNewSegment(message.DefaultSegment("MSH"));
        }

        /// <summary>
        /// Serialize to MLLP escaped byte array
        /// </summary>
        /// <param name="validate">Optional. Validate the message before serializing</param>
        /// <returns>MLLP escaped byte array</returns>
        public byte[] GetMLLP(bool validate = false)
        {
            string hl7 = this.SerializeMessage(validate);

            return MessageHelper.GetMLLP(hl7);
        }

        public override bool Equals(object? obj)
        {
            if ((obj is null) == false)
            {
                if (obj is Message)
                {
                    Message? msg = obj as Message;
                    return this.Equals(msg!.HL7Message);
                }

                if (obj is string)
                {
                    string? str = obj as string;
                    str = (str == null) ? string.Empty : str;
                    var arr1 = MessageHelper.SplitString(this.HL7Message, this.Encoding.SegmentDelimiter, StringSplitOptions.RemoveEmptyEntries);
                    var arr2 = MessageHelper.SplitString(str!, this.Encoding.SegmentDelimiter, StringSplitOptions.RemoveEmptyEntries);

                    return arr1.SequenceEqual(arr2);
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the components of a value's position descriptor
        /// </summary>
        /// <returns>A boolean indicating whether all the components are valid or not</returns>
        private static bool ValidateValueFormat(IList<string> allComponents)
        {
            bool isValid = false;

            if (allComponents.Count > 0)
            {
                if (Regex.IsMatch(allComponents[0], SegmentRegex))
                {
                    for (int i = 1; i < allComponents.Count; i++)
                    {
                        if (i == 1 && Regex.IsMatch(allComponents[i], FieldRegex))
                        {
                            isValid = true;
                        }
                        else if (i > 1 && Regex.IsMatch(allComponents[i], OtherRegEx))
                        {
                            isValid = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        private static Field GetField(Segment segment, string index)
        {
            int repetition = 0;
            var matches = System.Text.RegularExpressions.Regex.Matches(index, FieldRegex);

            if (matches.Count < 1)
            {
                throw new HL7Exception("Invalid Field Index", HL7Exception.BAD_MESSAGE);
            }

            int fieldIndex = int.Parse(matches[0].Groups[1].Value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
            fieldIndex--;

            if (matches[0].Length > 3)
            {
                repetition = int.Parse(matches[0].Groups[3].Value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
                repetition--;
            }

            var field = segment.FieldList[fieldIndex];

            if (field.HasRepetitions)
            {
                field = field.RepetitionList[repetition];
            }

            return field;
        }

        /// <summary>
        /// Builds an ACK or NACK message for this message
        /// </summary>
        /// <param name="code">ack code like AA, AR, AE</param>
        /// <param name="isNack">true for generating a NACK message, otherwise false</param>
        /// <param name="errMsg">error message to be sent with NACK</param>
        /// <returns>An ACK or NACK message if success, otherwise null</returns>
        private Message? CreateAckMessage(string code, bool isNack, string? errMsg)
        {
            var response = new StringBuilder();

            if (this.MessageStructure != "ACK")
            {
                var dateString = MessageHelper.LongDateWithFractionOfSecond(DateTime.Now);
                var msh = this.SegmentList["MSH"].First();
                var delim = this.Encoding.FieldDelimiter;

                response.Append("MSH").Append(this.Encoding.AllDelimiters).Append(delim).Append(msh.FieldList[4].Value).Append(delim).Append(msh.FieldList[5].Value).Append(delim)
                    .Append(msh.FieldList[2].Value).Append(delim).Append(msh.FieldList[3].Value).Append(delim)
                    .Append(dateString).Append(delim).Append(delim).Append("ACK").Append(delim).Append(this.MessageControlID).Append(delim)
                    .Append(this.ProcessingID).Append(delim).Append(this.Version).Append(this.Encoding.SegmentDelimiter);

                response.Append("MSA").Append(delim).Append(code).Append(delim).Append(this.MessageControlID).Append(isNack ? delim + errMsg! : string.Empty).Append(this.Encoding.SegmentDelimiter);
            }
            else
            {
                return null;
            }

            try
            {
                var message = new Message(response.ToString());
                message.ParseMessage();
                return message;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        /// <summary>
        /// Validates the HL7 message for basic syntax
        /// </summary>
        /// <returns>A boolean indicating whether the whole message is valid or not</returns>
        private bool ValidateMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.HL7Message))
                {
                    // Check message length - MSH+Delimeters+12Fields in MSH
                    if (this.HL7Message.Length < 20)
                    {
                        throw new HL7Exception("Message Length too short: " + this.HL7Message.Length + " chars.", HL7Exception.BAD_MESSAGE);
                    }

                    // Check if message starts with header segment
                    if (!this.HL7Message.StartsWith("MSH", StringComparison.Ordinal))
                    {
                        throw new HL7Exception("MSH segment not found at the beggining of the message", HL7Exception.BAD_MESSAGE);
                    }

                    this.Encoding.EvaluateSegmentDelimiter(this.HL7Message);
                    this.HL7Message = string.Join(this.Encoding.SegmentDelimiter, MessageHelper.SplitMessage(this.HL7Message)) + this.Encoding.SegmentDelimiter;

                    // Check Segment Name & 4th character of each segment
                    char fourthCharMSH = this.HL7Message[3];
                    this.allSegments = MessageHelper.SplitMessage(this.HL7Message);

                    foreach (string strSegment in this.allSegments)
                    {
                        if (string.IsNullOrWhiteSpace(strSegment))
                        {
                            continue;
                        }

                        string segmentName = strSegment.Substring(0, 3);
                        bool isValidSegmentName = System.Text.RegularExpressions.Regex.IsMatch(segmentName, SegmentRegex);

                        if (!isValidSegmentName)
                        {
                            throw new HL7Exception("Invalid segment name found: " + strSegment, HL7Exception.BAD_MESSAGE);
                        }

                        if (strSegment.Length > 3 && fourthCharMSH != strSegment[3])
                        {
                            throw new HL7Exception("Invalid segment found: " + strSegment, HL7Exception.BAD_MESSAGE);
                        }
                    }

                    string fieldDelimitersMessage = this.allSegments[0].Substring(3, 8 - 3);
                    this.Encoding.EvaluateDelimiters(fieldDelimitersMessage);

                    // Count field separators, MSH.12 is required so there should be at least 11 field separators in MSH
                    int countFieldSepInMSH = this.allSegments[0].Count(f => f == this.Encoding.FieldDelimiter);

                    if (countFieldSepInMSH < 11)
                    {
                        throw new HL7Exception("MSH segment doesn't contain all the required fields", HL7Exception.BAD_MESSAGE);
                    }

                    // Find Message Version
                    var mshFields = MessageHelper.SplitString(this.allSegments[0], this.Encoding.FieldDelimiter);

                    if (mshFields.Count >= 12)
                    {
                        this.Version = MessageHelper.SplitString(mshFields[11], this.Encoding.ComponentDelimiter)[0];
                    }
                    else
                    {
                        throw new HL7Exception("HL7 version not found in the MSH segment", HL7Exception.REQUIRED_FIELD_MISSING);
                    }

                    // Find Message Type & Trigger Event
                    try
                    {
                        string msh9 = mshFields[8];

                        if (!string.IsNullOrEmpty(msh9))
                        {
                            var msh9Comps = MessageHelper.SplitString(msh9, this.Encoding.ComponentDelimiter);

                            if (msh9Comps.Count >= 3)
                            {
                                this.MessageStructure = msh9Comps[2];
                            }
                            else if (msh9Comps.Count > 0 && msh9Comps[0] != null
                                && string.Equals(msh9Comps[0], "ACK", StringComparison.Ordinal) == true)
                            {
                                this.MessageStructure = "ACK";
                            }
                            else if (msh9Comps.Count == 2)
                            {
                                this.MessageStructure = msh9Comps[0] + "_" + msh9Comps[1];
                            }
                            else if (msh9Comps.Count == 1 && msh9Comps[0] != null && msh9Comps[0].StartsWith("Z", StringComparison.Ordinal))
                            {
                                // A custom MessageType might not have a event type code.
                                this.MessageStructure = msh9Comps[0];
                            }
                            else
                            {
                                throw new HL7Exception("Message Type & Trigger Event value not found in message", HL7Exception.UNSUPPORTED_MESSAGE_TYPE);
                            }
                        }
                        else
                        {
                            throw new HL7Exception("MSH.10 not available", HL7Exception.UNSUPPORTED_MESSAGE_TYPE);
                        }
                    }
                    catch (System.IndexOutOfRangeException e)
                    {
                        throw new HL7Exception("Can't find message structure (MSH.9.3) - " + e.Message, HL7Exception.UNSUPPORTED_MESSAGE_TYPE);
                    }

                    try
                    {
                        this.MessageControlID = mshFields[9];

                        if (string.IsNullOrEmpty(this.MessageControlID))
                        {
                            throw new HL7Exception("MSH.10 - Message Control ID not found", HL7Exception.REQUIRED_FIELD_MISSING);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HL7Exception("Error occurred while accessing MSH.10 - " + ex.Message, HL7Exception.REQUIRED_FIELD_MISSING);
                    }

                    try
                    {
                        this.ProcessingID = mshFields[10];

                        if (string.IsNullOrEmpty(this.ProcessingID))
                        {
                            throw new HL7Exception("MSH.11 - Processing ID not found", HL7Exception.REQUIRED_FIELD_MISSING);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HL7Exception("Error occurred while accessing MSH.11 - " + ex.Message, HL7Exception.REQUIRED_FIELD_MISSING);
                    }
                }
                else
                {
                    throw new HL7Exception("No Message Found", HL7Exception.BAD_MESSAGE);
                }
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Failed to validate the message with error - " + ex.Message, HL7Exception.BAD_MESSAGE);
            }

            return true;
        }

        /// <summary>
        /// Serializes a field into a string with proper this.Encoding
        /// </summary>
        private void SerializeField(Field field, StringBuilder strMessage)
        {
            if (field.ComponentList.Count > 0)
            {
                int indexCom = 0;

                foreach (Component com in field.ComponentList)
                {
                    indexCom++;
                    if (com.SubComponentList.Count > 0)
                    {
                        strMessage.Append(string.Join(this.Encoding.SubComponentDelimiter.ToString(), com.SubComponentList.Select(sc => this.Encoding.Encode(sc.Value))));
                    }
                    else
                    {
                        strMessage.Append(this.Encoding.Encode(com.Value));
                    }

                    if (indexCom < field.ComponentList.Count)
                    {
                        strMessage.Append(this.Encoding.ComponentDelimiter);
                    }
                }
            }
            else
            {
                strMessage.Append(this.Encoding.Encode(field.Value));
            }
        }

        /// <summary>
        /// Get all segments in order as they appear in original message. This the usual order: IN1|1 IN2|1 IN1|2 IN2|2.
        /// </summary>
        /// <returns>A list of segments in the proper order</returns>
        private List<Segment> GetAllSegmentsInOrder()
        {
            List<Segment> list = new List<Segment>();

            foreach (string segName in this.SegmentList.Keys)
            {
                foreach (Segment seg in this.SegmentList[segName])
                {
                    list.Add(seg);
                }
            }

            return list.OrderBy(o => o.SequenceNo).ToList();
        }
    }
}
