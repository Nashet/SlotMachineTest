using Socket.WebSocket4Net.Default;
using Socket.WebSocket4Net.System.Linq;

namespace Socket.WebSocket4Net.Protocol
{
	internal class ProtocolProcessorFactory
	{
		private IProtocolProcessor[] m_OrderedProcessors;

		public ProtocolProcessorFactory(params IProtocolProcessor[] processors)
		{
			this.m_OrderedProcessors = processors
			  .OrderByDescending<IProtocolProcessor, int>(p => (int)p.Version)
			  .ToArray<IProtocolProcessor>();
		}

		public IProtocolProcessor GetProcessorByVersion(WebSocketVersion version)
		{
			return m_OrderedProcessors.FirstOrDefault<IProtocolProcessor>(
			   p => p.Version == version);
		}

		public IProtocolProcessor GetPreferedProcessorFromAvialable(int[] versions)
		{
			foreach (int num in versions.OrderByDescending<int, int>(i => i))
			{
				foreach (IProtocolProcessor orderedProcessor in this.m_OrderedProcessors)
				{
					int version = (int)orderedProcessor.Version;
					if (version >= num)
					{
						if (version <= num)
							return orderedProcessor;
					}
					else
						break;
				}
			}

			return null;
		}
	}
}