using System;
using log4net;

namespace IntroToMocks.HandRolled.Tests
{
   public class FakeLog:ILog
   {
      public bool LastCallWasDebug { get; set; }
      public object LastMessage { get; set; }

      #region ILog Members

      public void Debug(object message, Exception exception)
      {
         throw new NotImplementedException();
      }

      public void Debug(object message)
      {
         this.LastCallWasDebug = true;
         this.LastMessage = message;
      }

      public void DebugFormat(IFormatProvider provider, string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void DebugFormat(string format, object arg0, object arg1, object arg2)
      {
         throw new NotImplementedException();
      }

      public void DebugFormat(string format, object arg0, object arg1)
      {
         throw new NotImplementedException();
      }

      public void DebugFormat(string format, object arg0)
      {
         throw new NotImplementedException();
      }

      public void DebugFormat(string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void Error(object message, Exception exception)
      {
         throw new NotImplementedException();
      }

      public void Error(object message)
      {
         throw new NotImplementedException();
      }

      public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void ErrorFormat(string format, object arg0, object arg1, object arg2)
      {
         throw new NotImplementedException();
      }

      public void ErrorFormat(string format, object arg0, object arg1)
      {
         throw new NotImplementedException();
      }

      public void ErrorFormat(string format, object arg0)
      {
         throw new NotImplementedException();
      }

      public void ErrorFormat(string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void Fatal(object message, Exception exception)
      {
         throw new NotImplementedException();
      }

      public void Fatal(object message)
      {
         throw new NotImplementedException();
      }

      public void FatalFormat(IFormatProvider provider, string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void FatalFormat(string format, object arg0, object arg1, object arg2)
      {
         throw new NotImplementedException();
      }

      public void FatalFormat(string format, object arg0, object arg1)
      {
         throw new NotImplementedException();
      }

      public void FatalFormat(string format, object arg0)
      {
         throw new NotImplementedException();
      }

      public void FatalFormat(string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void Info(object message, Exception exception)
      {
         throw new NotImplementedException();
      }

      public void Info(object message)
      {
         throw new NotImplementedException();
      }

      public void InfoFormat(IFormatProvider provider, string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void InfoFormat(string format, object arg0, object arg1, object arg2)
      {
         throw new NotImplementedException();
      }

      public void InfoFormat(string format, object arg0, object arg1)
      {
         throw new NotImplementedException();
      }

      public void InfoFormat(string format, object arg0)
      {
         throw new NotImplementedException();
      }

      public void InfoFormat(string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public bool IsDebugEnabled
      {
         get { throw new NotImplementedException(); }
      }

      public bool IsErrorEnabled
      {
         get { throw new NotImplementedException(); }
      }

      public bool IsFatalEnabled
      {
         get { throw new NotImplementedException(); }
      }

      public bool IsInfoEnabled
      {
         get { throw new NotImplementedException(); }
      }

      public bool IsWarnEnabled
      {
         get { throw new NotImplementedException(); }
      }

      public void Warn(object message, Exception exception)
      {
         throw new NotImplementedException();
      }

      public void Warn(object message)
      {
         throw new NotImplementedException();
      }

      public void WarnFormat(IFormatProvider provider, string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      public void WarnFormat(string format, object arg0, object arg1, object arg2)
      {
         throw new NotImplementedException();
      }

      public void WarnFormat(string format, object arg0, object arg1)
      {
         throw new NotImplementedException();
      }

      public void WarnFormat(string format, object arg0)
      {
         throw new NotImplementedException();
      }

      public void WarnFormat(string format, params object[] args)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region ILoggerWrapper Members

      public log4net.Core.ILogger Logger
      {
         get { throw new NotImplementedException(); }
      }

      #endregion
   }
}
