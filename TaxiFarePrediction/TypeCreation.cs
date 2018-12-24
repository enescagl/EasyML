using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;

namespace TaxiFarePrediction
{
    public static class TypeCreation
    {
        public static void CreateType(string TypeName, List<TextLoader.Column> PropertyColumns)
        {
            AssemblyName TypeAssemblyName = new AssemblyName($"Dynamic{TypeName}Assembly");
            AssemblyBuilder TypeAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(TypeAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder TypeModule = TypeAssemblyBuilder.DefineDynamicModule($"{TypeName}Module");
            TypeBuilder Type = TypeModule.DefineType(TypeName, TypeAttributes.Public | TypeAttributes.Class);
            ConstructorInfo ctor = typeof(ColumnAttribute).GetConstructor()
            CustomAttributeBuilder Attribute = new CustomAttributeBuilder()
            for (int index = 0; index < PropertyColumns.Count; index++)
            {
                PropertyBuilder mlTypeProperties = Type.DefineProperty(
                    PropertyColumns[index].Name, ,
                    CallingConventions.Any,
                    PropertyColumns[index].Type.Value.ToType(),
                    new Type[] { PropertyColumns[index].Type.Value.ToType() }
                );
            }

        }

    }
}