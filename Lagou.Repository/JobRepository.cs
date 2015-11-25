﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lagou.Repository.Entity;

namespace Lagou.Repository
{
    public class JobRepository
    {
        private DapperHelper dapperHelper = new DapperHelper();
        public void Insert(JobEntity entity)
        {
            string sql = @"INSERT  INTO Job
                            ( Score ,CreateTime ,FormatCreateTime ,PositionId ,PositionName ,PositionType ,WorkYear ,Education ,
                            JobNature ,CompanyName ,CompanyId ,City ,CompanyLogo ,IndustryField ,PositionAdvantag ,Salary ,PositionFirstType ,
                            LeaderName ,CompanySize ,FinanceStage)
                            VALUES  ( @Score ,@CreateTime ,@FormatCreateTime ,@PositionId ,@PositionName ,@PositionType ,@WorkYear ,@Education ,
                            @JobNature ,@CompanyName ,@CompanyId ,@City ,@CompanyLogo ,@IndustryField ,@PositionAdvantag ,@Salary ,@PositionFirstType ,
                            @LeaderName ,@CompanySize ,@FinanceStage)";

            using (DbConnection conn = dapperHelper.GetConnection())
            {
                conn.Open();
                conn.Execute(sql, entity);
            }
        }

        public void Insert(List<JobEntity> entity)
        {
            string sql = @"INSERT  INTO Job
                            ( Score ,CreateTime ,FormatCreateTime ,PositionId ,PositionName ,PositionType ,WorkYear ,Education ,
                            JobNature ,CompanyName ,CompanyId ,City ,CompanyLogo ,IndustryField ,PositionAdvantag ,Salary ,PositionFirstType ,
                            LeaderName ,CompanySize ,FinanceStage)
                            VALUES  ( @Score ,@CreateTime ,@FormatCreateTime ,@PositionId ,@PositionName ,@PositionType ,@WorkYear ,@Education ,
                            @JobNature ,@CompanyName ,@CompanyId ,@City ,@CompanyLogo ,@IndustryField ,@PositionAdvantag ,@Salary ,@PositionFirstType ,
                            @LeaderName ,@CompanySize ,@FinanceStage)";

            using (DbConnection conn = dapperHelper.GetConnection())
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(sql, entity, tran, 20);
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 城市职位需求总数
        /// </summary>
        /// <returns></returns>
        public List<CityCompanyJobEntity> QueryJCityNeedJobNum()
        {
            var jobList = new List<CityCompanyJobEntity>();
            string sql = @"SELECT TOP 15
                                COUNT(City) [JobNum] ,
                                City 
                        FROM    dbo.Job
                        GROUP BY City 
                        ORDER BY JobNum DESC";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                jobList = conn.Query<CityCompanyJobEntity>(sql).ToList();
            }

            return jobList;
        }

        /// <summary>
        /// 查询城市的公司总数
        /// </summary>
        /// <returns></returns>
        public List<CityCompanyJobEntity> QueryCityCompanyNum()
        {
            var companyList = new List<CityCompanyJobEntity>();
            string sql = @"SELECT City,COUNT(DISTINCT CompanyName) AS CompanyNum
                        FROM Job
                        GROUP BY City  ORDER BY CompanyNum DESC ";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                companyList = conn.Query<CityCompanyJobEntity>(sql).ToList();
            }

            return companyList;
        }
        /// <summary>
        /// 查询城市某职位的需求数
        /// </summary>
        /// <param name="positionName"></param>
        /// <returns></returns>
        public List<CityCompanyJobEntity> QueryPositionNum(string positionName)
        {
            var list = new List<CityCompanyJobEntity>();

            string sql = string.Format(@"SELECT COUNT(*)[JobNum],city FROM Job WHERE PositionName ='{0}'
                            GROUP BY City",positionName);
            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                list = conn.Query<CityCompanyJobEntity>(sql).ToList();
            }

            return list;
        }

        /// <summary>
        ///各城市薪水
        /// </summary>
        /// <param name="positionName"></param>
        /// <returns></returns>
        public List<WorkYearSalaryEntity> QueryPositionNameSalary(string positionName)
        {

            string sql = string.Format(@"SELECT  COUNT(*) [JobNum] ,
                                City ,
                                Salary
                        FROM    dbo.Job
                        WHERE   PositionName = '{0}'
                        GROUP BY City ,
                                Salary
                        ORDER BY City",positionName);




            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                return  conn.Query<WorkYearSalaryEntity>(sql).ToList();
            }

        }


        /// <summary>
        /// 各城市对不同年限段的人数需求
        /// </summary>
        /// <param name="positionName"></param>
        /// <returns></returns>
        public List<WorkYearSalaryEntity> QueryWorkYearJobNum()
        {
            string sql = @"SELECT  COUNT(*) [JobNum] ,
                                City ,
                                WorkYear
                        FROM    dbo.Job
                        GROUP BY City ,
                                WorkYear
                        ORDER BY City";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                return conn.Query<WorkYearSalaryEntity>(sql).ToList();
            }
        }



        /// <summary>
        ///行业薪水分布
        /// </summary>
        /// <returns></returns>
        public List<IndustrySalarEntity> QueryIndustrySalary()
        {

            string sql = @"select COUNT(*)[Num], IndustryField[Industry] from Job
                            group by IndustryField
                            order by Num desc ";
            using (var conn =dapperHelper.GetConnection())
            {
                conn.Open();
               return  conn.Query<IndustrySalarEntity>(sql).ToList();
            }


        }

        public List<IndustrySalarEntity> QueryIndustrySalary(string industryName)
        {

            string sql = string.Format(@"select COUNT(*)[Num],Salary from Job  where IndustryField like '%{0}%'    
  
                        group by Salary",industryName);
            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                return conn.Query<IndustrySalarEntity>(sql).ToList();
            }
        }

        /// <summary>
        ///公司融资阶段
        /// </summary>
        /// <returns></returns>
        public List<FinanceStageEntity> QueryFinanceStage()
        {
            string sql = @"select COUNT(*)[Num],FinanceStage from Job 
                            group by FinanceStage order by Num desc";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                return conn.Query<FinanceStageEntity>(sql).ToList();
            }
        }

        public List<FinanceStageSalaryEntity> QueryFinanceStageSalary(string financeStage,string positionName="",string city ="")
        {
            //TODO:防SQL注入
            string condition = string.Empty;
            if (!string.IsNullOrEmpty(positionName) && !string.IsNullOrEmpty(city))
            {
                condition = string.Format(" AND PositionName='{0}' AND City='{1}'", positionName, city);

            }
            string sql = string.Format(@"select COUNT(*)[Num],Salary from Job 
                        where FinanceStage='{0}' {1} 
                        group by Salary",financeStage, condition);
            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                return conn.Query<FinanceStageSalaryEntity>(sql).ToList();

            }
        }

        /// <summary>
        /// 不同融资阶段 对不同年限人才的需求
        /// </summary>
        /// <returns></returns>
        public List<FinanceStageWorkYearEntity> QueryFinanceStageWorkYear()
        {
            string sql = @"select COUNT(*)[Num],FinanceStage,WorkYear from Job
                            group by FinanceStage, WorkYear
                              order by FinanceStage";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                return conn.Query<FinanceStageWorkYearEntity>(sql).ToList();
            }

        }



        // 同一职位不同城市，不同的年限 的薪水差异

        //薪水城市年限的差异

        //城市对哪个年限的需求是最大的


        //每个年限，每个城市的平均薪水



    }
}
