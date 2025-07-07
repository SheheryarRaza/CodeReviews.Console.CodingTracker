using System.Data;
using Dapper;

namespace CodingTracker.SheheryarRaza
{
    public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
    {
        public override TimeSpan Parse(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return TimeSpan.Zero;
            }

            return TimeSpan.Parse((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, TimeSpan value)
        {
            parameter.Value = value.ToString("hh\\:mm\\:ss");
            parameter.DbType = DbType.String;
        }
    }
}
