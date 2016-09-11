using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Core.Signing.Crypto;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplyGame.Ethereum
{
    [FunctionOutput]
    public class TopScore
    {
        [Parameter("address", "addr", 1)]
        public string Address { get; set; }
        [Parameter("int", "score", 2)]
        public int Score { get; set; }
    }
    
    public class GameScoreService
    {
        public static string DEFAULT_MORDEN = "https://morden.infura.io/aEcNY6wGN4KuEpoXQRxZ";
       
        //public static string PRIVATE_KEY = "0x822b4de0c646385ab8d1a29313e01a31c50d79e634809b4c90e67b00a4328401";
        public static string PRIVATE_KEY = "0xaf98a1bdf2140578318e2c5e7d5956a3ee0a6732090c2991a9166a6639ad368f";
      
        private string abi = "[{'constant':false,'inputs':[{'name':'score','type':'int256'}],'name':'setTopScore','outputs':[],'type':'function'},{'constant':true,'inputs':[{'name':'','type':'uint256'}],'name':'topScores','outputs':[{'name':'addr','type':'address'},{'name':'score','type':'int256'}],'type':'function'},{'constant':false,'inputs':[],'name':'getCountTopScores','outputs':[{'name':'','type':'uint256'}],'type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'userTopScores','outputs':[{'name':'','type':'int256'}],'type':'function'}]";
        private string contractAddress = "0x87B7A7D7c81EC400b29aA59EFAB915a9493f6e94";
        private int topScoreCache = -1;

        public static GameScoreService Current { get; private set; }

        public static void Init(string privateKey, string url)
        {
            Current = new GameScoreService(privateKey, url);
        }

        public string PrivateKey { get; private set; }
        public string Url { get; private set; }
        private Web3 web3;
        private string address;
        private Contract contract;

        private GameScoreService(string privateKey, string url)
        {
            this.PrivateKey = privateKey;
            this.Url = url;
            this.web3 = new Web3(url);
            this.address = "0x" + EthECKey.GetPublicAddress(privateKey); //could do checksum
            this.contract = web3.Eth.GetContract(abi, contractAddress);
        }


        public async Task<int> GetUserTopScore()
        {
            if (topScoreCache > 0) return topScoreCache;
            var function = contract.GetFunction("userTopScores");
            topScoreCache =  await function.CallAsync<int>(address).ConfigureAwait(false);
            return topScoreCache;
        }

        public async Task<string> SubmitScoreAsync(int score)
        {
            var userTopScore = await GetUserTopScore();
            if(score > userTopScore)
            {
                topScoreCache = score;

                var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address).ConfigureAwait(false);

                var function = contract.GetFunction("setTopScore");
                var data = function.GetData(score);
                var encoded = web3.OfflineTransactionSigning.SignTransaction(PrivateKey, contractAddress, 0,
                  txCount.Value, 1000000000000L, 900000, data);
                return await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(encoded).ConfigureAwait(false);
            }
            return null;
        }

        public async Task<List<TopScore>> GetTopScoresAsync()
        {
            var countFunction = contract.GetFunction("getCountTopScores");
            var count = await countFunction.CallAsync<int>().ConfigureAwait(false);

            var topScoresFunction = contract.GetFunction("topScores");

            var scores = new List<TopScore>();
            for(int i=0; i< count; i++)
            {
                var topScore = await topScoresFunction.CallDeserializingToObjectAsync<TopScore>(i).ConfigureAwait(false);
                scores.Add(topScore);
            }

            return scores.OrderByDescending(x => x.Score).ToList();
        }
    }
}
