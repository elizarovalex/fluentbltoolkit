using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.Mapping;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	/// <summary>
	/// Конфигуратор инфраструктур BLToolkit для поддержки fluent настройки мапинга
	/// </summary>
	public static class FluentConfig
	{
		private static Dictionary<Assembly, List<TypeExtension>> _hash = new Dictionary<Assembly, List<TypeExtension>>();

		/// <summary>
		/// Получить настройки мапинга из сборки содержащей указанный тип
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<TypeExtension> GetMapingFromAssemblyOf<T>()
		{
			return GetMapingFromAssembly(typeof(T).Assembly);
		}

		/// <summary>
		/// Получить настройки мапинга из указанной сборки
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<TypeExtension> GetMapingFromAssembly(Assembly assembly)
		{
			List<TypeExtension> res;
			if (!_hash.TryGetValue(assembly, out res))
			{
				res = new List<TypeExtension>();
				_hash.Add(assembly, res);

				string fluentType = typeof(IFluentMap).FullName;
				res.AddRange(from type in assembly.GetTypes()
							 where type.IsClass && !type.IsAbstract
							 && (null != type.GetInterface(fluentType)) // Реализован нужный интерфейс
							 && (null != type.GetConstructor(new Type[0])) // Есть конструктор без параметров
							 select ((IFluentMap)Activator.CreateInstance(type)).Map());
			}
			return res;
		}

		/// <summary>
		/// Сконфигурировать DbManager
		/// </summary>
		/// <param name="dbManager"></param>
		/// <param name="extensions"></param>
		public static void Configure(DbManager dbManager, IEnumerable<TypeExtension> extensions)
		{
			MappingSchema mappingSchema = dbManager.MappingSchema ?? (dbManager.MappingSchema = Map.DefaultSchema);
			Configure(mappingSchema, extensions);
		}

		/// <summary>
		/// Сконфигурировать DataProvider
		/// </summary>
		/// <param name="dataProvider"></param>
		/// <param name="extensions"></param>
		public static void Configure(DataProviderBase dataProvider, IEnumerable<TypeExtension> extensions)
		{
			MappingSchema mappingSchema = dataProvider.MappingSchema ?? (dataProvider.MappingSchema = Map.DefaultSchema);
			Configure(mappingSchema, extensions);
		}

		/// <summary>
		/// Сконфигурировать MappingSchema
		/// </summary>
		/// <param name="mappingSchema"></param>
		/// <param name="extensions"></param>
		public static void Configure(MappingSchema mappingSchema, IEnumerable<TypeExtension> extensions)
		{
			ExtensionList extensionList = mappingSchema.Extensions ?? (mappingSchema.Extensions = new ExtensionList());
			Configure(extensionList, extensions);
		}

		/// <summary>
		/// Сконфигурировать ExtensionList
		/// </summary>
		/// <param name="extensionList"></param>
		/// <param name="extensions"></param>
		public static void Configure(ExtensionList extensionList, IEnumerable<TypeExtension> extensions)
		{
			foreach (TypeExtension typeExtension in extensions)
			{
				extensionList.Add(typeExtension);
			}
		}
	}
}