using System.Reflection;


namespace Juga.Data.Dynamic;

public static  class DynamicConvertorExtension
{
    public static DynamicQuery ConvertToFilter<T>(this T queryModel,  IList<Sort>? sort = null , params SpecifiedFilter[] specifiedFilters  ) where T : class
    {
        var recordType = typeof(T);
        var properties = recordType.GetProperties().Where(x=>x.GetValue(queryModel)!=null).ToArray();
          
        List<SpecifiedFilter> specifiedListOfFilters=null;
        if (specifiedFilters.Length > 0)
        {
            specifiedListOfFilters = specifiedFilters.ToList();
                
        }

        if (properties.Length > 0)
        {
               
            var firstProperty = properties[0];
               
            var firstFilter = CreateDefaultFilter(queryModel, firstProperty, specifiedListOfFilters);


            List<Filter> additionalFilters = new ();

                
            for (var i = 1; i < properties.Length; i++)
            {
                var filter = CreateDefaultFilter(queryModel, properties[i], specifiedListOfFilters);
                additionalFilters.Add(filter);
            }

            // Set the list of additional filters to the Filters property of the first filter
            firstFilter.Filters = additionalFilters;
            //if (specifiedFilters.Length > 0)
            //{
            //     specifiedListOfFilters = specifiedFilters.ToList();
            //    if (specifiedListOfFilters?.Count > 0)
            //    {
            //        if (specifiedFilters.Select(x => x.Field).Contains(firstFilter.Field))
            //        {
            //            SpecifyProperty<T>(specifiedListOfFilters, firstFilter);
            //        }

            //        if (firstFilter.Filters.Any())
            //        {
            //            foreach (var filter in firstFilter.Filters)
            //            {
            //                SpecifyProperty<T>(specifiedListOfFilters, filter);
            //            }
            //        }

            //    }
            //}
                
            var dynamicQuery = new DynamicQuery
            {
                Filter = firstFilter,
                Sort = sort
            };
            return dynamicQuery;
        }
          
        return null;
    }

    private static Filter CreateDefaultFilter<T>(T queryModel, PropertyInfo firstProperty, List<SpecifiedFilter> specifiedListOfFilters=null) where T : class
    {

        var @operator = "eq";
        var logic = "and";
        var op = specifiedListOfFilters?.FirstOrDefault(x => x.Field == firstProperty.Name);
        if (op != null)
        {
            @operator = op.Operator;
            logic=op.Logic;
        }
        return new Filter
        {
            Field = firstProperty.Name,                          // Set the property name
            Value = firstProperty.GetValue(queryModel)?.ToString(), // Set the value of the property
            Operator = @operator,                                     // Set the operator to "eq"
            Logic = logic                                        // Set the logic to "and"
        };
    }

    private static void SpecifyProperty<T>(List<SpecifiedFilter> specifiedFilters, Filter firstFilter) where T : class
    {
        var specifieldFilter = specifiedFilters.FirstOrDefault(x => x.Field == firstFilter.Field);
        if (specifieldFilter != null)
        {
            firstFilter.Operator = specifieldFilter.Operator;
            firstFilter.Logic = specifieldFilter.Logic;
        }
    }
}