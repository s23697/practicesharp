// 
// Copyright (c) 2004-2010 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

#if !NET_CF

namespace NLog.LayoutRenderers
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using NLog.Config;
    using NLog.Internal;

    /// <summary>
    /// The call site (class name, method name and source information).
    /// </summary>
    [LayoutRenderer("callsite")]
    [ThreadAgnostic]
    public class CallSiteLayoutRenderer : LayoutRenderer, IUsesStackTrace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallSiteLayoutRenderer" /> class.
        /// </summary>
        public CallSiteLayoutRenderer()
        {
            this.ClassName = true;
            this.MethodName = true;
#if !SILVERLIGHT
            this.FileName = false;
            this.IncludeSourcePath = true;
#endif
        }

        /// <summary>
        /// Gets or sets a value indicating whether to render the class name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool ClassName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render the method name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool MethodName { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets a value indicating whether to render the source file name and line number.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include source file path.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(true)]
        public bool IncludeSourcePath { get; set; }
#endif

        /// <summary>
        /// Gets the level of stack trace information required by the implementing class.
        /// </summary>
        StackTraceUsage IUsesStackTrace.StackTraceUsage
        {
            get
            {
#if !SILVERLIGHT
                if (this.FileName)
                {
                    return StackTraceUsage.Max;
                }
#endif

                return StackTraceUsage.WithoutSource;
            }
        }

        /// <summary>
        /// Renders the call site and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            StackFrame frame = logEvent.UserStackFrame;
            if (frame != null)
            {
                MethodBase method = frame.GetMethod();
                if (this.ClassName)
                {
                    if (method.DeclaringType != null)
                    {
                        builder.Append(method.DeclaringType.FullName);
                    }
                    else
                    {
                        builder.Append("<no type>");
                    }
                }

                if (this.MethodName)
                {
                    if (this.ClassName)
                    {
                        builder.Append(".");
                    }

                    if (method != null)
                    {
                        builder.Append(method.Name);
                    }
                    else
                    {
                        builder.Append("<no method>");
                    }
                }

#if !SILVERLIGHT
                if (this.FileName)
                {
                    string fileName = frame.GetFileName();
                    if (fileName != null)
                    {
                        builder.Append("(");
                        if (this.IncludeSourcePath)
                        {
                            builder.Append(fileName);
                        }
                        else
                        {
                            builder.Append(Path.GetFileName(fileName));
                        }

                        builder.Append(":");
                        builder.Append(frame.GetFileLineNumber());
                        builder.Append(")");
                    }
                }
#endif
            }
        }
    }
}

#endif