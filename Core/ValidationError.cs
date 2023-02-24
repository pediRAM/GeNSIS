namespace GeNSIS.Core
{
    #region Usings
    using System.ComponentModel;
    #endregion Usings

    /// <summary>
    /// todo: implement and comment type: ValidationError !
    /// Provides:
    /// <para>Events: <see cref="PropertyChanged"/>.</para>
    /// <para>Properties: <see cref="Name"/>, <see cref="Error"/>, <see cref="Hint"/>.</para>
    /// </summary>
    public class ValidationError
    {

        #region Constructors
        public ValidationError(string pName, string pError, string pHint)
        {
            Name = pName;
            Error = pError;
            Hint = pHint;
        }
        #endregion Constructors

        #region Properties
        public string Name { get; }

        public string Error { get; }

        public string Hint { get; }

        #endregion Properties
    }
}
