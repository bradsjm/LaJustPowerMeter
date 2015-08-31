/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Event Arguments used for passing a device id
    /// </summary>
    public class DeviceIdEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        /// <value>The device id.</value>
        public DeviceId DeviceId {get; set;}
    }

    /// <summary>
    /// Represents the 3 byte device Id of a Hogu, Target, etc.
    /// </summary>
    public class DeviceId
    {
        /// <summary>
        /// Gets or sets the first ID byte.
        /// </summary>
        /// <value>The Id1.</value>
        public byte ID1 { get; private set; }
        /// <summary>
        /// Gets or sets the second ID byte.
        /// </summary>
        /// <value>The Id2.</value>
        public byte ID2 { get; private set; }
        /// <summary>
        /// Gets or sets the third ID byte.
        /// </summary>
        /// <value>The Id3.</value>
        public byte ID3 { get; private set; }
        /// <summary>
        /// String representation of ID
        /// </summary>
        private string _idstr;

        /// <summary>
        /// Initializes a new invalid instance of the <see cref="DeviceId"/> class.
        /// </summary>
        public DeviceId() 
        {
            _idstr = "INVALID";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceId"/> class.
        /// </summary>
        /// <param name="id1">The first id byte.</param>
        /// <param name="id2">The second id byte..</param>
        /// <param name="id3">The third id byte.</param>
        public DeviceId(byte id1, byte id2, byte id3)
        {
            ID1 = id1;
            ID2 = id2;
            ID3 = id3;
            _idstr = System.BitConverter.ToString(new byte[] { ID1, ID2, ID3 });
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _idstr;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _idstr.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to DeviceId return false.
            DeviceId p = obj as DeviceId;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (p.ID1 == this.ID1 && p.ID2 == this.ID2 && p.ID3 == this.ID3);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DeviceId a, DeviceId b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (a.ID1 == b.ID1 && a.ID2 == b.ID2 && a.ID3 == b.ID3);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DeviceId a, DeviceId b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid()
        {
            return !(ID1 == 0 && ID2 == 0 && ID3 == 0);
        }
    }
}
