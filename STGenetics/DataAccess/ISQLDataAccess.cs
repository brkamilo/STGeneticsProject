namespace STGenetics.DataAccess
{
    public interface ISQLDataAccess
    {
        Task<IEnumerable<T>> GetData<T, Param>(string spName, Param parameters, string connectionId = "conn");
        Task SaveData<Param>(string spName, Param parameters, string connectionId = "conn");
        Task<IEnumerable<T>> GetDataByQuery<T, Param>(string query, Param parameters, string connection = "conn");
    }
}
