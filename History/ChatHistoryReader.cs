using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.History
{
    /// <summary>
    /// czytaj wszystko z history
    /// </summary>
    internal class ChatHistoryReader
    {
        public void Read(ChatHistory history, Guid historyUuid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tutaj trzeba się skupić np na qdrant albo jakimś nosql
        /// po interfejsie powiedzmy IDataSourceReader albo IQueryExecutor 
        /// gdzie bedzie posiadac funkcje do szukania po uuid, autorze, dacie, itp
        /// jakos to sprytnie wykonac
        /// </summary>
        /// <param name="history"></param>
        /// <param name="historyUuid"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ReadExternal(ChatHistory history, Guid historyUuid)
        {
            throw new NotImplementedException();
        }
    }
}
