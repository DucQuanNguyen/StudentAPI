using Microsoft.Data.SqlClient;

public interface IParameterAdder<T>
{
    void AddParameters(SqlCommand cmd, T entity, bool includeId = true);
}