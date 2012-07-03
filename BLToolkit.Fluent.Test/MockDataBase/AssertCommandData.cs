using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLToolkit.Fluent.Test.MockDataBase
{
	public class AssertCommandData
	{
		private readonly MockCommandData _data;

		public AssertCommandData(MockCommandData data)
		{
			_data = data;
		}

		public void AreField(string fieldName, string message = null)
		{
			if (!_data.Fields.Contains(fieldName))
			{
				Assert.Fail(message ?? string.Format("Fail field '{0}'", fieldName));
			}
		}

		public void AreNotField(string fieldName, string message = null)
		{
			if (_data.Fields.Contains(fieldName))
			{
				Assert.Fail(message ?? string.Format("Fail field '{0}'", fieldName));
			}
		}

		public void AreTable(string tableName, string message = null)
		{
			if (!_data.Tables.Contains(tableName))
			{
				Assert.Fail(message ?? string.Format("Fail table '{0}'", tableName));
			}
		}
	}
}