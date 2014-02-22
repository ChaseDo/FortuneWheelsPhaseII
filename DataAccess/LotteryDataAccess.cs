using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace DataAccess
{
    public class LotteryDataAccess
    {
        public static DataTable GetLotteryHistory(string sPhoneNumber)
        {
            DataBase dataBase = new DataBase();
            string sql = "select a.cellphoneno,a.lotterytime,b.awardname,a.cardcode from LotteryHistory a join Awards b on a.AwardId = b.AwardId";
            if (!string.IsNullOrWhiteSpace(sPhoneNumber))
            {
                sql += "where CellPhoneNo = " + sPhoneNumber;
            }
            return dataBase.ExcuteSqlReturnDataTable(sql);
        }

        public static DataTable GetAllLotteryAwards()
        {
            DataBase dataBase = new DataBase();
            string sql = "select * from Awards";
            return dataBase.ExcuteSqlReturnDataTable(sql);
        }

        public static DataTable GetLotteryUser(string sPhoneNumber)
        {
            DataBase dataBase = new DataBase();
            string sql = "select * from Players where CellPhoneNo = @CellPhoneNo";
            SqlParameter[] sqlparameter = new SqlParameter[1];
            sqlparameter[0] = new SqlParameter("@CellPhoneNo", sPhoneNumber);
            return dataBase.ExcuteSqlReturnDataTable(sql, sqlparameter);
        }

        public static DataTable GetLotteryAllUser()
        {
            DataBase dataBase = new DataBase();
            string sql = "select * from Players ";
            return dataBase.ExcuteSqlReturnDataTable(sql);
        }

        public static void StoreLotteryUser(string sPhoneNumber)
        {
            DataBase dataBase = new DataBase();
            string sql = "exec spNewPlayer @CellPhoneNo = @CellPhone";
            SqlParameter[] sqlparameter = new SqlParameter[1];
            sqlparameter[0] = new SqlParameter("@CellPhone", sPhoneNumber);
            dataBase.ExcuteSqlReturnInt(sql, sqlparameter);
        }

        public static DataTable ReduceCountAndSaveHistory(string sPhoneNumber, string sAwardId)
        {
            DataBase dataBase = new DataBase();
            string sql = "exec spLotteryProcess @CellPhoneNo = @CellPhone , @AwardID = @Award ";
            SqlParameter[] sqlparameter = new SqlParameter[2];
            sqlparameter[0] = new SqlParameter("@CellPhone", sPhoneNumber);
            sqlparameter[1] = new SqlParameter("@Award", sAwardId);
            return dataBase.ExcuteSqlReturnDataTableTransaction(sql, sqlparameter);
        }

        public static bool UpdateAward(string sAwardId,string sAwardName, string sRate, string sTotalCount, string sSurplusCount)
        {
            DataBase dataBase = new DataBase();
            string sql = "update Awards set AwardName = @AwardName ,Rate = @Rate, TotalCount = @TotalCount,SurplusCount = @SurplusCount where AwardId = @AwardId ";
            SqlParameter[] sqlparameter = new SqlParameter[5];
            sqlparameter[0] = new SqlParameter("@AwardName", sAwardName);
            sqlparameter[1] = new SqlParameter("@Rate", sRate);
            sqlparameter[2] = new SqlParameter("@TotalCount", sTotalCount);
            sqlparameter[3] = new SqlParameter("@SurplusCount", sSurplusCount);
            sqlparameter[4] = new SqlParameter("@AwardId", sAwardId);
            return dataBase.ExcuteSqlReturnInt(sql, sqlparameter) > 0;
        }

        //public static void test()
        //{
        //    DataBase dataBase = new DataBase();
        //    List<int> list = new List<int>() { 1, 2, 5, 10 };
        //    int temp = 0;
        //    for (int i = 0; i < 10000; i++)
        //    {
        //        Guid guid = Guid.NewGuid();
        //        Random random = new Random();
        //        temp = random.Next(0, 4);
        //        string sRandomString = Convert.ToBase64String(guid.ToByteArray());
        //        string sql = "INSERT INTO [dbo].[CardCodes] ([AwardID],[CardCode],[Used])VALUES(" + list[temp] + ",'" + sRandomString + "',0)";
        //        dataBase.ExcuteSqlReturnInt(sql);
        //    }
        //}
    }
}
