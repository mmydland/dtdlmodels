using System;
using System.Threading.Tasks;

namespace dotnetValidate
{
    class Program
    {

        /// <summary>
        /// Validates a given set of models to make sure that 
        ///     1. All needed dtmi models are present
        ///     2. No duplicate model IDs exist in the public repo
        ///     3. The set of models parse correctly and are valid
        /// </summary>
        /// <param name="files"> Files to be including in the validation run</param>
        /// <returns></returns>
        static async Task<int> Main(params string[] files)
        {
            if (files == null || files.Length == 0)
            {
                Console.WriteLine("No filenames were passed.");
                Console.WriteLine("Run dotnet_validate -h for help");
                return 1;
            }
            try
            {
                ModelValidator validator = new ModelValidator(files);
                await validator.ValidateModels();
                if (!validator.IsValidModel)
                {
                    Console.WriteLine(validator.ValidationErrors);
                    return 1;
                }
                Console.WriteLine("Model is valid");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
