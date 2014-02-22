using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace Entity
{
    public static class Helper
    {
        public static List<T> ConvertToModel<T>(this DataTable dataTable) where T : new()
        {
            List<T> ts = new List<T>();

            List<PropertyInfo> properties = GetPropertiesInDataTable<T>(dataTable);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                ts.Add(PopulateObject<T>(properties, dataRow));
            }

            return ts;
        }

        private static List<PropertyInfo> GetPropertiesInDataTable<T>(DataTable dataTable) where T : new()
        {
            List<PropertyInfo> propertys = new List<PropertyInfo>(typeof(T).GetProperties());
            propertys.RemoveAll(p => !(p.CanWrite && dataTable.Columns.Contains(p.Name)));
            return propertys;
        }

        private static T PopulateObject<T>(List<PropertyInfo> properties, DataRow dataRow) where T : new()
        {
            T newObject = new T();

            string stringValue;

            foreach (PropertyInfo property in properties)
            {
                object value = dataRow[property.Name];

                if (value != DBNull.Value)
                {
                    stringValue = value.ToString();
                    switch (property.PropertyType.Name)
                    {
                        case "Double":
                            property.SetValue(newObject, Decimal.ToDouble(Convert.ToDecimal(stringValue)), null);
                            break;
                        case "Float":
                            property.SetValue(newObject, (float)Decimal.ToDouble(Convert.ToDecimal(stringValue)), null);
                            break;
                        case "Int32":
                            property.SetValue(newObject, Convert.ToInt32(stringValue), null);
                            break;
                        case "Single":
                            property.SetValue(newObject, Convert.ToSingle(stringValue), null);
                            break;
                        case "Boolean":
                            property.SetValue(newObject, Convert.ToBoolean(stringValue), null);
                            break;
                        case "DateTime":
                            property.SetValue(newObject, Convert.ToDateTime(stringValue), null);
                            break;
                        default:
                            property.SetValue(newObject, stringValue, null);
                            break;
                    }
                }
            }
            return newObject;
        }
    }
}
