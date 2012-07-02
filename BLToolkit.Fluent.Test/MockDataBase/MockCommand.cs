using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BLToolkit.Fluent.Test.MockDataBase
{
	public partial class MockDb
	{
		private partial class MockCommand : IDbCommand
		{
			private readonly MockDb _db;

			public MockCommand(MockDb db)
			{
				_db = db;
				Parameters = new DataParameterCollection();
			}

			public void Dispose()
			{
			}

			public void Prepare()
			{
			}

			public void Cancel()
			{
			}

			public IDbDataParameter CreateParameter()
			{
				return new MockDbDataParameter();
			}

			public int ExecuteNonQuery()
			{
				return MockCommandData().NonQueryResult;
			}

			public IDataReader ExecuteReader()
			{
				return new MockReader(MockCommandData(false));
			}

			public IDataReader ExecuteReader(CommandBehavior behavior)
			{
				return ExecuteReader();
			}

			public object ExecuteScalar()
			{
				return MockCommandData().ScalarResult;
			}

			public IDbConnection Connection { get; set; }

			public IDbTransaction Transaction { get; set; }

			public string CommandText { get; set; }

			public int CommandTimeout { get; set; }

			public CommandType CommandType { get; set; }

			public IDataParameterCollection Parameters { get; private set; }

			public UpdateRowSource UpdatedRowSource { get; set; }

			private MockCommandData MockCommandData(bool isUsing = true)
			{
				var cmd = _db.NextCommand();
				cmd.Parameters = Parameters.Cast<MockDbDataParameter>().ToList();
				cmd.CommandText = CommandText;
				cmd.IsUsing = isUsing;
				return cmd;
			}

			private class DataParameterCollection : List<MockDbDataParameter>, IDataParameterCollection
			{
				public bool Contains(string parameterName)
				{
					return this.Any(p => p.ParameterName == parameterName);
				}

				public int IndexOf(string parameterName)
				{
					for (int i = 0; i < Count; i++)
					{
						if (this[i].ParameterName == parameterName)
						{
							return i;
						}
					}
					return -1;
				}

				public void RemoveAt(string parameterName)
				{
					var data = this.FirstOrDefault(p => p.ParameterName == parameterName);
					if (null != data)
					{
						Remove(data);
					}
				}

				public object this[string parameterName]
				{
					get
					{
						var data = this.FirstOrDefault(p => p.ParameterName == parameterName);
						return null == data ? null : data.Value;
					}
					set
					{
						var data = this.FirstOrDefault(p => p.ParameterName == parameterName);
						if (null == data)
						{
							Add(new MockDbDataParameter { ParameterName = parameterName, Value = value });
						}
						else
						{
							data.Value = value;
						}
					}
				}
			}
		}
	}
}