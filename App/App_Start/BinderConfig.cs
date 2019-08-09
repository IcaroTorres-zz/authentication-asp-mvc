using ModelBinders = App.Models.Binder.ModelBinders;

namespace App
{
    public class BinderConfig
    {
        public static void RegisterBinders()
        {
            // Binders
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(decimal), new ModelBinders.Binder<decimal>());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(int), new ModelBinders.Binder<int>());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(long), new ModelBinders.Binder<long>());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(int?), new ModelBinders.Binder<int?>());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(long?), new ModelBinders.Binder<long?>());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(decimal?), new ModelBinders.Binder<decimal?>());
        }
    }
}