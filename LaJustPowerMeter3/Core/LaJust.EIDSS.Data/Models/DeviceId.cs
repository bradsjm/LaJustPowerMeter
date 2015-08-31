// <copyright file="DeviceId.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Represents the 3 byte device Id of a Hogu, Target, etc.
    /// </summary>
    public struct DeviceId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceId"/> class.
        /// </summary>
        /// <param name="id1">The first id byte.</param>
        /// <param name="id2">The second id byte..</param>
        /// <param name="id3">The third id byte.</param>
        public DeviceId(byte id1, byte id2, byte id3) : this()
        {
            this.ID1 = id1;
            this.ID2 = id2;
            this.ID3 = id3;
        }

        /// <summary>
        /// Gets the first ID byte.
        /// </summary>
        /// <value>The Id1 value.</value>
        public byte ID1 { get; set; }

        /// <summary>
        /// Gets the second ID byte.
        /// </summary>
        /// <value>The Id2 value.</value>
        public byte ID2 { get; set; }

        /// <summary>
        /// Gets the third ID byte.
        /// </summary>
        /// <value>The Id3 value.</value>
        public byte ID3 { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is DeviceId)
            {
                // Return true if the fields match
                return this.ID1 == ((DeviceId)obj).ID1 &&
                       this.ID2 == ((DeviceId)obj).ID2 &&
                       this.ID3 == ((DeviceId)obj).ID3;
            }
            return false;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return System.BitConverter.ToString(new byte[] { this.ID1, this.ID2, this.ID3 }); ;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid()
        {
            return !(this.ID1 == 0 && this.ID2 == 0 && this.ID3 == 0);
        }
    }
}
