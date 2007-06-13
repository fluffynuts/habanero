using Chillisoft.Crypt.v2;
using Chillisoft.Generic.v2;

namespace Chillisoft.Bo.v2
{
    /// <summary>
    /// Manages a password string that is to be encrypted or decrypted using the
    /// Blowfish encryption algorithm. For the purposes of information 
    /// security, passwords are usually stored in a database in their 
    /// encrypted form, hence the need for this encryption facility.
    /// </summary>
    public class PasswordBlowfish : CustomProperty
    {
        private string itsEncryptedValue;
        private static Crypter itsCrypter;

        /// <summary>
        /// Constructor to initialise a new password
        /// </summary>
        /// <param name="value">The data to encrypt</param>
        /// <param name="isLoading">Whether the value has already been
        /// encrypted (for instance, if an encrypted value is being loaded
        /// from the database)</param>
        /// <exception cref="HabaneroApplicationException">Thrown if the
        /// data to encrypt is not a string type</exception>
        /// TODO ERIC - why not just make the parameter a string? and not
        /// throw an exception
        /// - could rename isLoading to isEncrypted
        public PasswordBlowfish(object value, bool isLoading) : base(value, isLoading)
        {
            if (value is string)
            {
                if (isLoading)
                {
                    itsEncryptedValue = (string) value;
                }
                else
                {
                    itsEncryptedValue = Encrypt((string) value);
                }
            }
            else
            {
                throw new HabaneroApplicationException("A password must be of type string");
            }
        }

        /// <summary>
        /// Returns the encryption device, which in this case uses the Blowfish
        /// algorithm
        /// </summary>
        protected static Crypter Crypter
        {
            get
            {
                if (itsCrypter == null)
                {
                    itsCrypter = new BlowfishCrypter();
                }
                return itsCrypter;
            }
        }

        /// <summary>
        /// Encrypts a given string
        /// </summary>
        /// <param name="value">The string to encrypt</param>
        /// <returns>Returns the encrypted string</returns>
        private string Encrypt(string value)
        {
            return Crypter.EncryptString(value);
        }

        /// <summary>
        /// Decrypts a given string
        /// </summary>
        /// <param name="value">The string to decrypt</param>
        /// <returns>Returns the decrypted string</returns>
        private string Decrypt(string value)
        {
            return Crypter.DecryptString(value);
        }

        /// <summary>
        /// Returns the value that has been encrypted
        /// </summary>
        public object Value
        {
            get { return itsEncryptedValue; }
        }

        /// <summary>
        /// Returns the encrypted value in its decrypted form
        /// </summary>
        public string DecryptedValue
        {
            get { return this.Decrypt(itsEncryptedValue); }
        }

        /// <summary>
        /// Indicates whether the contents of the given password object
        /// equal the contents of this object
        /// </summary>
        /// <param name="obj">The password object to compare with</param>
        /// <returns>Returns true if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is PasswordBlowfish)
            {
                PasswordBlowfish password = (PasswordBlowfish) obj;
                return password.DecryptedValue.Equals(this.DecryptedValue);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hashcode of the decrypted value held
        /// </summary>
        /// <returns>Returns a hashcode integer</returns>
        public override int GetHashCode()
        {
            return this.DecryptedValue.GetHashCode();
        }

        /// <summary>
        /// Returns the encrypted value as a string
        /// </summary>
        /// <returns>Returns a string</returns>
        public override string ToString()
        {
            return itsEncryptedValue;
        }

        /// <summary>
        /// Returns the encrypted value that is to be persisted to the database.
        /// </summary>
        /// <returns>Returns the encrypted value</returns>
        public override object GetPersistValue()
        {
            return itsEncryptedValue;
        }
    }
}