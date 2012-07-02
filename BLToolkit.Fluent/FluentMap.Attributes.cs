namespace BLToolkit.Fluent
{
	/// <summary>
	/// Поддержка возможности настройки мапинга из кода без использования аттрибутов
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class FluentMap<T>
	{
		private static class Attributes
		{
			public static class TableName
			{
				public const string Name = "TableName";
				public const string Database = "DatabaseName";
				public const string Owner = "OwnerName";
			}
			public static class MapField
			{
				public const string MapName = "MapField";
				public const string Storage = "FieldStorage";
				public const string IsInheritanceDiscriminator = "IsInheritanceDiscriminator";
			}
			public static class PrimaryKey
			{
				public const string Order = "PrimaryKey";
			}
			public static class SqlIgnore
			{
				public const string Ignore = "SqlIgnore";
			}
			public static class MapIgnore
			{
				public const string Ignore = "MapIgnore";
			}
			public const string NonUpdatable = "NonUpdatable";
			public const string Identity = "Identity";
			public const string Trimmable = "Trimmable";
		}
	}
}