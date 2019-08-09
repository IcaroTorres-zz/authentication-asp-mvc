using System;
using System.Globalization;
using System.Web.Mvc;

namespace App.Models.Binder
{
    public class ModelBinders
    {
        public class Binder<T> : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext,
                                    ModelBindingContext bindingContext)
            {
                object value = null;
                var modelState = new ModelState
                {
                    Value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName)
                };

                try
                {
                    var attemptedValue = typeof(T) == typeof(string) ? modelState.Value.AttemptedValue.Trim()
                                                                     : modelState.Value.AttemptedValue;

                    value = Convert.ChangeType(attemptedValue, typeof(T), CultureInfo.CurrentCulture);
                }
                catch (FormatException e) { modelState.Errors.Add(e); }

                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
                return value;
            }
        }
    }
}