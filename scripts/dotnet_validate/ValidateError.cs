using System;

namespace dotnetValidate
{
    public class ValidateError
    {
        private string _name, _id, _description;
        public string Name => _name;

        public string Id => _id;

        public string Description => _description;

        internal ValidateError(string name, string id, string description)
        {
            _name = name;
            _id = id;
            _description = description;
        }
      
    }
}