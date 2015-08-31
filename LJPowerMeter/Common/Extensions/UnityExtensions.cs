namespace LaJust.PowerMeter.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;

    public static class UnityExtensions
    {
        /// <summary>
        /// Unity Extension to provide type tracking
        /// </summary>
        public class UnityExtensionWithTypeTracking : UnityContainerExtension
        {
            internal readonly List<RegisterInstanceEventArgs> registeredInstanceTypes = new List<RegisterInstanceEventArgs>();
            internal readonly List<RegisterEventArgs> registeredTypes = new List<RegisterEventArgs>();

            /// <summary>
            /// Initial the container with this extension's functionality.
            /// </summary>
            /// <remarks>
            /// When overridden in a derived class, this method will modify the given
            /// <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
            /// install it's functions into the container.</remarks>
            protected override void Initialize()
            {
                Context.RegisteringInstance += (sender, e) => registeredInstanceTypes.Add(e);
                Context.Registering += (sender, e) => registeredTypes.Add(e);
            }
        }

        /// <summary>
        /// Determines whether this instance can resolve the specified container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can resolve the specified container; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanResolve<T>(this IUnityContainer container)
        {
            return (container.Configure<UnityExtensionWithTypeTracking>().registeredInstanceTypes.Any(t => t.RegisteredType == typeof(T)) ||
                container.Configure<UnityExtensionWithTypeTracking>().registeredTypes.Any(t => t.TypeFrom == typeof(T)));
        }

        /// <summary>
        /// Determines whether this instance can resolve the specified container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can resolve the specified container; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanResolve<T>(this IUnityContainer container, string name)
        {
            return (container.Configure<UnityExtensionWithTypeTracking>().registeredInstanceTypes
                .Any(t => t.RegisteredType == typeof(T) && t.Name != null && t.Name.Equals(name, StringComparison.InvariantCulture))
                ||
                container.Configure<UnityExtensionWithTypeTracking>().registeredTypes
                .Any(t => t.TypeFrom == typeof(T) && t.Name != null && t.Name.Equals(name, StringComparison.InvariantCulture)));
        }
    }

}
