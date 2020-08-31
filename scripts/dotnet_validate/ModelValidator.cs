using System.Text;
using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.DigitalTwins.Parser;

namespace dotnetValidate
{
    public class ModelValidator
    {
        private static ModelParser modelParser = new ModelParser();
        private Dictionary<string, string> prModelFiles = new Dictionary<string, string>();
        private IList<ValidateError> validateErrors = new List<ValidateError>();

        public string ValidationErrors
        {
            get
            {
                if (validateErrors.Count() == 0)
                {
                    return String.Empty;
                }

                int errNum = 1;
                StringBuilder sb = new StringBuilder($"Validation failed with {validateErrors.Count} error(s):\n");
                foreach (var err in validateErrors)
                {
                    sb.Append($"Error {errNum}\n");
                    sb.Append($"\tName: {err.Name}\n");
                    sb.Append($"\tId: {err.Id}\n");
                    sb.Append($"\tDescrption: {err.Description}\n");
                    errNum++;
                }
                return sb.ToString();
            }
        }

        public bool IsValidModel { get { return validateErrors.Count == 0; } }

        public ModelValidator(IEnumerable<string> jsonLdFilePaths)
        {
            modelParser.Options.Add(ModelParsingOption.AllowUndefinedExtensions);
            modelParser.DtmiResolver = DtmiResolver;
            foreach (var filePath in jsonLdFilePaths)
            {
                var model = File.ReadAllText(filePath);
                prModelFiles.Add(filePath, File.ReadAllText(filePath));
            }
        }

        private async Task FindDuplicatePublicModels()
        {
            foreach ((var filename, var rawModel) in prModelFiles)
            {
                (bool isValid, var result) = await ParseModels(rawModel);

                var dtmiIfc = result.Where(result => result.Value.EntityKind == DTEntityKind.Interface).Select(result => result.Key.AbsoluteUri).ToList();

                foreach (var dtmiUri in dtmiIfc)
                {
                    // check current model doesn't exist in public repo
                    if (await ModelRepo.IsModelInPublicRepo(dtmiUri))
                    {
                        validateErrors.Add(new ValidateError("Duplicate model found in model repo", dtmiUri, $"The file {filename} contains DTMI {dtmiUri} which already exists in the public repo"));
                        continue;
                    }

                }
            }
        }

        public async Task<bool> ValidateModels()
        {
            await FindDuplicatePublicModels();

            try
            {
                await modelParser.ParseAsync(prModelFiles.Values.ToArray());
            }
            catch (ResolutionException rex)
            {
                validateErrors.Add(new ValidateError(rex.Message, string.Join("\n\t", rex.UndefinedIdentifiers), rex.Message));
            }
            catch (ParsingException pex)
            {
                foreach (var err in pex.Errors)
                {
                    validateErrors.Add(new ValidateError(err.Message, err.Property, err.Cause));
                }
            }
            return IsValidModel;
        }

        public async Task<(bool, IReadOnlyDictionary<Dtmi, DTEntityInfo>)> ParseModels(params string[] models)
        {
            IReadOnlyDictionary<Dtmi, DTEntityInfo> result;
            try
            {
                result = await modelParser.ParseAsync(models);
                return (true, result);
            }
            catch (Exception ex)
            {
                return (false, new Dictionary<Dtmi, DTEntityInfo>());
            }
        }

        private async Task<IEnumerable<string>> DtmiResolver(IReadOnlyCollection<Dtmi> dtmis)
        {
            List<String> jsonLds = new List<string>();

            foreach (var dtmi in dtmis)
            {
                string model = await ModelRepo.GetMissingModel(dtmi.AbsoluteUri);
                if (!String.IsNullOrWhiteSpace(model))
                {
                    jsonLds.Add(model);
                }
            }
            return jsonLds;
        }
    }
}