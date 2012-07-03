using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLToolkit.Fluent.Test.MockDataBase
{
	/// <summary>
	/// DB configure start point
	/// </summary>
	public partial class MockDb : IDbConnection
	{
		private int _cmdIndex = -1;
		private readonly List<MockCommandData> _commands = new List<MockCommandData>();

		public List<MockCommandData> Commands { get { return _commands; } }

		private MockCommandData NextCommand()
		{
			_cmdIndex++;
			if (_cmdIndex == _commands.Count)
			{
				Assert.Fail("Command not define");
			}
			return _commands[_cmdIndex];
		}

		public MockDb NewReader(params string[] fields)
		{
			var data = new MockReaderData();
			data.SetNames(fields);
			CurrentSetupCommandData = new MockCommandData { ReaderResult = data };
			return this;
		}

		public MockDb NewRow(params object[] values)
		{
			CurrentSetupCommandData.ReaderResult.Values.Add(values);
			return this;
		}

		private MockCommandData CurrentSetupCommandData
		{
			get { return _commands.LastOrDefault(); }
			set { _commands.Add(value); }
		}

		public MockDb NewNonQuery(int value = 1)
		{
			CurrentSetupCommandData = new MockCommandData { NonQueryResult = value };
			return this;
		}
	}
}