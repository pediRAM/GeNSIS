namespace GeNSIS.Core
{
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
