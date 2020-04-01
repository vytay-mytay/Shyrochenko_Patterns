using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.RequestModels;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Exporting
{
    public interface IExportService
    {
        /// <summary>
        /// Export user's table to file
        /// </summary>
        /// <param name="format">Output file format</param>
        /// <param name="order">Sorting model</param>
        /// <returns>File as bytes array</returns>
        Task<byte[]> ExportUsersTable(ExportFormat format, OrderingRequestModel<UserTableColumn, SortingDirection> order);
    }
}
