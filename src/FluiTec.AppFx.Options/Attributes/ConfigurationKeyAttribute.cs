﻿using System;

namespace FluiTec.AppFx.Options.Attributes
{
    /// <summary>	Attribute for configuration name. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationKeyAttribute : Attribute
    {
        #region Properties

        /// <summary>	Gets or sets the name. </summary>
        /// <value>	The name of the entity. </value>
        public string Name { get; set; }

        #endregion

        #region Constructors

        /// <summary>	Default constructor. </summary>
        public ConfigurationKeyAttribute()
        {
        }

        /// <summary>	Constructor. </summary>
        /// <param name="name">	The name of the configuration. </param>
        public ConfigurationKeyAttribute(string name)
        {
            Name = name;
        }

        #endregion
    }
}
