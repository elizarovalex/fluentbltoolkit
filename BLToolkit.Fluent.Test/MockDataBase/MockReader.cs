using System;
using System.Data;
using System.Linq;

namespace BLToolkit.Fluent.Test.MockDataBase
{
	public partial class MockDb
	{
		private partial class MockCommand : IDbCommand
		{
			private class MockReader : IDataReader
			{
				private readonly MockReaderData _data;
				private int _rowIndex = -1;
				private MockCommandData _cmd;

				public MockReader(MockCommandData data)
				{
					_cmd = data;
					_data = data.ReaderResult;
				}

				public void Dispose()
				{
				}

				public string GetName(int i)
				{
					return _data.Names[i];
				}

				public string GetDataTypeName(int i)
				{
					throw new NotImplementedException();
				}

				public Type GetFieldType(int i)
				{
					Type type = _data.Types[i];
					if (null == type)
					{
						object o = _data.Values.First()[i];
						if (null == o)
						{
							throw new ArgumentException();
						}
						type = o.GetType();
					}
					return type;
				}

				public object GetValue(int i)
				{
					return _data.Values[_rowIndex][i];
				}

				public int GetValues(object[] values)
				{
					_data.Values[_rowIndex].CopyTo(values, 0);
					return Math.Min(_data.Values[_rowIndex].Length, values.Length);
				}

				public int GetOrdinal(string name)
				{
					throw new NotImplementedException();
				}

				public bool GetBoolean(int i)
				{
					return Convert.ToBoolean(_data.Values[_rowIndex][i]);
				}

				public byte GetByte(int i)
				{
					return Convert.ToByte(_data.Values[_rowIndex][i]);
 				}

				public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
				{
					throw new NotImplementedException();
				}

				public char GetChar(int i)
				{
					return Convert.ToChar(_data.Values[_rowIndex][i]);
				}

				public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
				{
					throw new NotImplementedException();
				}

				public Guid GetGuid(int i)
				{
					throw new NotImplementedException();
				}

				public short GetInt16(int i)
				{
					return Convert.ToInt16(_data.Values[_rowIndex][i]);
				}

				public int GetInt32(int i)
				{
					return Convert.ToInt32(_data.Values[_rowIndex][i]);
				}

				public long GetInt64(int i)
				{
					return Convert.ToInt64(_data.Values[_rowIndex][i]);
				}

				public float GetFloat(int i)
				{
					throw new NotImplementedException();
				}

				public double GetDouble(int i)
				{
					return Convert.ToDouble(_data.Values[_rowIndex][i]);
				}

				public string GetString(int i)
				{
					return Convert.ToString(_data.Values[_rowIndex][i]);
				}

				public decimal GetDecimal(int i)
				{
					return Convert.ToDecimal(_data.Values[_rowIndex][i]);
				}

				public DateTime GetDateTime(int i)
				{
					return Convert.ToDateTime(_data.Values[_rowIndex][i]);
				}

				public IDataReader GetData(int i)
				{
					throw new NotImplementedException();
				}

				public bool IsDBNull(int i)
				{
					return null == _data.Values[_rowIndex][i];
				}

				public int FieldCount
				{
					get { return _data.Names.Count; }
				}

				object IDataRecord.this[int i]
				{
					get { throw new NotImplementedException(); }
				}

				object IDataRecord.this[string name]
				{
					get { throw new NotImplementedException(); }
				}

				public void Close()
				{
					IsClosed = true;
				}

				public DataTable GetSchemaTable()
				{
					throw new NotImplementedException();
				}

				public bool NextResult()
				{
					var index = _rowIndex + 1;
					return _data.Values.Count > index;
				}

				public bool Read()
				{
					_cmd.IsUsing = true;
					_rowIndex++;
					return _data.Values.Count > _rowIndex;
				}

				public int Depth
				{
					get { throw new NotImplementedException(); }
				}

				public bool IsClosed { get; set; }

				public int RecordsAffected
				{
					get { return _rowIndex + 1; }
				}
			}
		}
	}
}