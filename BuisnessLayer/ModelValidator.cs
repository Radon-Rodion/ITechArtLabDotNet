using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BuisnessLayer
{
    public class ModelValidator
    {
        public static IList<ValidationResult> ValidateViewModel(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

            return result;
        }

        public static string StringifyErrors(IList<ValidationResult> errors)
        {
            StringBuilder errorsString = new StringBuilder("");
            foreach (var error in errors)
            {
                errorsString.Append($"{error.MemberNames.First()}: {error.ErrorMessage}").Append('\n');
            }
            
            return errorsString.ToString();
        }
    }
}
