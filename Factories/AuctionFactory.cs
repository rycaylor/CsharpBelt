using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using blackbelt.Models;
using System.Linq; 
using System;
using blackbelt.Factory;

namespace blackbelt.Factory
{
    public class AuctionFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        public AuctionFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;

        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }

        public void CreateAuction(Auction newAuction)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string query = @"INSERT INTO auctions (Name, Description, Bid, EndingDate, CreatedAt, UpdatedAt, UserId) VALUES (@Name, @Description, @Bid, @EndingDate, NOW(), NOW(), @UserId)";
                dbConnection.Open();
                dbConnection.Execute(query, newAuction);
            }
        }
        public List<Auction> GrabAllAuctions()
        {
             using (IDbConnection dbConnection = Connection){
                string query = "SELECT * FROM auctions ORDER BY EndingDate;";
                dbConnection.Open();
                var auctions = dbConnection.Query<Auction>(query).ToList();

                foreach (var auction in auctions){
                    query = $"SELECT FirstName FROM users WHERE (Id = {auction.UserId})";
                    User user = dbConnection.Query<User>(query).SingleOrDefault();
                    auction.TimeLeft = auction.EndingDate - DateTime.Now;
                    auction.User = user;
                }
                return auctions;
            }
        }

        public Auction GrabAuction(int id)
        {
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT * From auctions WHERE (id={id})";
                dbConnection.Open();
                var auction = dbConnection.Query<Auction>(query).SingleOrDefault();
                auction.User = dbConnection.Query<User>($"SELECT FirstName, LastName FROM users WHERE (Id = {auction.UserId})").SingleOrDefault();
                auction.TimeLeft = auction.EndingDate- DateTime.Now;
                return auction;
            }
        }
        public Bidder GrabCurrentBidder(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"SELECT * FROM bidders WHERE bidders.AuctionId = {id}";
                dbConnection.Open();
                var currentbid = dbConnection.Query<Bidder>(query).SingleOrDefault();
                if(currentbid != null){
                    currentbid.User = dbConnection.Query<User>($"SELECT FirstName FROM users WHERE (Id = {currentbid.UserId})").SingleOrDefault();
                }
                return currentbid;
            }            
        }
        public void DeleteAuction(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query3 = $"SELECT * FROM bidders WHERE (bidders.AuctionId = {id})";
                dbConnection.Open();
                var currentbid = dbConnection.Query<Bidder>(query3).SingleOrDefault();
                if(currentbid != null)
                {
                    string query = $"DELETE FROM bidders WHERE (bidders.AuctionId = {id})";
                    dbConnection.Execute(query);
                }
                string query2 = $"DELETE FROM auctions WHERE (Id = {id})";
                dbConnection.Execute(query2);

            }
        }

        public void UpdateBid(Bidder updateBid)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"DELETE FROM bidders WHERE (AuctionId = {updateBid.AuctionId})";
                dbConnection.Open();
                dbConnection.Execute(query);
                query = @"INSERT INTO bidders (AuctionId, UserId) VALUES (@AuctionId, @UserId)";
                dbConnection.Execute(query, updateBid);
                query = $"UPDATE auctions SET Bid = {updateBid.Bid} WHERE (Id = {updateBid.AuctionId})";
                dbConnection.Execute(query);
            }
        }

        public void CompleteAuction(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"SELECT * FROM bidders WHERE (bidders.AuctionId = {id})";
                dbConnection.Open();
                var currentbid = dbConnection.Query<Bidder>(query).SingleOrDefault();
                if(currentbid != null){
                    string query2 = $"SELECT Id, FirstName, LastName, UserName, Wallet FROM users WHERE (Id={currentbid.UserId})";
                    string query3 = $"SELECT * From auctions WHERE (Id={id})";
                    var user = dbConnection.Query<User>(query2).SingleOrDefault();
                    var auction = dbConnection.Query<Auction>(query3).SingleOrDefault();
                    string query4 = $"SELECT Id, FirstName, LastName, UserName, Wallet FROM users WHERE (Id={auction.UserId})";
                    var user2 = dbConnection.Query<User>(query4).SingleOrDefault();
                    user.Wallet -= auction.Bid;
                    user2.Wallet += auction.Bid;
                    string query5 = $"UPDATE users SET Wallet = {user.Wallet} WHERE (Id = {user.Id})";
                    dbConnection.Execute(query5);
                    string query6 = $"UPDATE users SET Wallet = {user2.Wallet} WHERE (Id = {user2.Id})";
                    dbConnection.Execute(query6);
                    string query7 = $"UPDATE auctions SET Bid = {0} WHERE (Id = {id})";
                    dbConnection.Execute(query7);
                }


            }
        }

    }
}