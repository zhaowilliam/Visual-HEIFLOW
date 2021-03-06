//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using Heiflow.Core;
using System;

namespace Heiflow.Core.Plugin
{
 	/// <summary>
	/// Base class to be derived by all plugins (loaded by PluginCompiler)
	/// Keep as light-weight as possible to keep plugin simple to write.
	/// </summary>
	public abstract class Plugin
	{
		/// <summary>
		/// Handle to the  Application object
		/// </summary>
        protected IApplication m_Application;

		/// <summary>
		/// The directory from which this plugin was loaded.
		/// </summary>
		protected string m_PluginDirectory;

		/// <summary>
		/// Plugin running flag (true while running, reset when exiting plugin)
		/// </summary>
		protected bool m_isLoaded;



		/// <summary>
		/// Reference to the main application object.  
		/// Deprecated: Use ParentApplication property instead!  
		/// TODO: Remove this to avoid name collision
		/// </summary>
        public virtual IApplication Application
		{
			get
			{
				return m_Application;
			}
		}

		/// <summary>
		/// Reference to the main application object.
		/// </summary>
        public virtual IApplication ParentApplication
		{
			get
			{
				return m_Application;
			}
		}

		/// <summary>
		/// The location this plugin was loaded from.
		/// </summary>
		public virtual string PluginDirectory
		{
			get
			{
				return m_PluginDirectory;
			}
		}

		/// <summary>
		/// Whether the plugin is currently running.
		/// </summary>
		public virtual bool IsLoaded
		{
			get
			{
				return m_isLoaded;
			}
		}

        public bool Visible 
        { get; set; }

		/// <summary>
		/// Load the plugin.  This is the plugin entry point.
		/// </summary>
		/// <param name="parent">The Application.</param>
		public virtual void Load()
		{
			// Override with plugin initialization code.
		}

		/// <summary>
		/// Unload the plugin. Plugins that modify  or 
		/// runs in background should override this method.
		/// </summary>
		public virtual void Unload()
		{
			// Override with plugin dispose code.
		}

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }
		/// <summary>
		/// Base class load, calls Load. 
		/// </summary>
		/// <param name="parent"></param>
        public virtual void PluginLoad(IApplication parent, string pluginDirectory)
		{
			if(m_isLoaded)
				// Already loaded
				return;
			m_Application = parent;
			m_PluginDirectory = pluginDirectory;
			Load();
			m_isLoaded = true;
		}

		/// <summary>
		/// Base class unload, calls Unload. 
		/// </summary>
		public virtual void PluginUnload()
		{
			Unload();
			m_isLoaded = false;
		}
	}
}
