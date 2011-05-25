using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Rainbow.ObjectFlow.Framework {
	/// <summary>
	/// EarlyExitException allows for Workflows to exit without throwing out
	/// </summary>
	public class EarlyExitException : ApplicationException {

		/// <summary>
		/// Initializes a new instance of the EarlyExitException class.
		/// </summary>
		public EarlyExitException() : base() { }
		/// <summary>
		/// Initializes a new instance of the EarlyExitException class.
		/// </summary>
		public EarlyExitException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the EarlyExitException class.
		/// </summary>
		protected EarlyExitException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		/// <summary>
		/// Initializes a new instance of the EarlyExitException class.
		/// </summary>
		public EarlyExitException(string message, Exception innerException) : base(message, innerException) { }
	}
}
