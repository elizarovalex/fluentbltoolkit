using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	/// <summary>
	/// Интерфейс для получения результата мапинга
	/// </summary>
	public interface IFluentMap
	{
		/// <summary>
		/// Получить результат мапинга
		/// </summary>
		/// <returns></returns>
		TypeExtension Map();
	}
}