/****************************************************************************
 * Copyright (c) 2015 ~ 2023 liangxiegame MIT License
 *
 * QFramework v1.0
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 *
 * Author:
 *  liangxie        https://github.com/liangxie
 *  soso            https://github.com/so-sos-so
 *
 * Contributor
 *  TastSong        https://github.com/TastSong
 *  京产肠饭         https://gitee.com/JingChanChangFan/hk_-unity-tools
 *  猫叔(一只皮皮虾)  https://space.bilibili.com/656352/
 *  New一天
 *  幽飞冷凝雪～冷 
 *
 * Community
 *  QQ Group: 623597263
 * Latest Update: 2023.9.26 15:14 support godot4.net
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
	// IArchitecture 接口定义了一个架构的基本行为和操作
	#region Architecture

	public interface IArchitecture
	{
		// 注册系统组件
		void RegisterSystem<T>(T system) where T : ISystem;
		
		// 注册模型组件
		void RegisterModel<T>(T model) where T : IModel;

		// 注册工具组件
		void RegisterUtility<T>(T utility) where T : IUtility;
		
		// 获取系统组件
		T GetSystem<T>() where T : class, ISystem;

		// 获取模型组件
		T GetModel<T>() where T : class, IModel;

		// 获取工具组件
		T GetUtility<T>() where T : class, IUtility;

		// 发送命令		
		void SendCommand<T>(T command) where T : ICommand;

		// 发送命令并返回结果
		TResult SendCommand<TResult>(ICommand<TResult> command);

		// 发送查询并返回结果
		TResult SendQuery<TResult>(IQuery<TResult> query);

		// 发送事件
		void SendEvent<T>() where T : new();
		void SendEvent<T>(T e);

		// 注册和注销事件
		IUnRegister RegisterEvent<T>(Action<T> onEvent);
		void UnRegisterEvent<T>(Action<T> onEvent);
	}

	// Architecture 抽象类提供了基本架构的实现
	public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
	{
		private bool mInited = false;

		private HashSet<ISystem> mSystems = new HashSet<ISystem>();

		private HashSet<IModel> mModels = new HashSet<IModel>();

		// 注册补丁的回调
		public static Action<T> OnRegisterPatch = architecture => { };

		protected static T mArchitecture;

		// 提供对外的接口
		public static IArchitecture Interface
		{
			get
			{
				// 确保架构已被初始化
				if (mArchitecture == null)
				{
					MakeSureArchitecture();
				}

				return mArchitecture;
			}
		}

		// 确保架构被正确初始化
		static void MakeSureArchitecture()
		{
			if (mArchitecture == null)
			{
				mArchitecture = new T();
				mArchitecture.Init();

				// 调用注册补丁
				OnRegisterPatch?.Invoke(mArchitecture);

				// 初始化所有模型
				foreach (var architectureModel in mArchitecture.mModels)
				{
					architectureModel.Init();
				}
				
				// 清理模型集合
				mArchitecture.mModels.Clear();

				// 初始化所有系统
				foreach (var architectureSystem in mArchitecture.mSystems)
				{
					architectureSystem.Init();
				}

				// 清理系统集合
				mArchitecture.mSystems.Clear();

				mArchitecture.mInited = true;
			}
		}

		// 子类必须实现的初始化方法
		protected abstract void Init();

		// IOC 容器，用于依赖注入
		private IOCContainer mContainer = new IOCContainer();

		// 注册系统组件
		public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
		{
			system.SetArchitecture(this);
			mContainer.Register<TSystem>(system);

			if (!mInited)
			{
				mSystems.Add(system);
			}
			else
			{
				system.Init();
			}
		}

		// 注册模型组件
		public void RegisterModel<TModel>(TModel model) where TModel : IModel
		{
			model.SetArchitecture(this);
			mContainer.Register<TModel>(model);

			if (!mInited)
			{
				mModels.Add(model);
			}
			else
			{
				model.Init();
			}
		}

		// 注册实用程序组件
		public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility =>
			mContainer.Register<TUtility>(utility);

		// 获取系统组件
		public TSystem GetSystem<TSystem>() where TSystem : class, ISystem => mContainer.Get<TSystem>();

		// 获取模型组件
		public TModel GetModel<TModel>() where TModel : class, IModel => mContainer.Get<TModel>();

		// 获取实用程序组件
		public TUtility GetUtility<TUtility>() where TUtility : class, IUtility => mContainer.Get<TUtility>();

		// 发送命令并获取结果
		public TResult SendCommand<TResult>(ICommand<TResult> command) => ExecuteCommand(command);

		// 发送命令
		public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand => ExecuteCommand(command);

		// 执行带结果的命令
		protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
		{
			command.SetArchitecture(this);
			return command.Execute();
		}

		// 执行无结果的命令
		protected virtual void ExecuteCommand(ICommand command)
		{
			command.SetArchitecture(this);
			command.Execute();
		}

		// 发送查询并获取结果
		public TResult SendQuery<TResult>(IQuery<TResult> query) => DoQuery<TResult>(query);

		// 执行查询的受保护方法
		protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
		{
			query.SetArchitecture(this);
			return query.Do();
		}

		// 事件系统的实例
		private TypeEventSystem mTypeEventSystem = new TypeEventSystem();

		// 发送事件
		public void SendEvent<TEvent>() where TEvent : new() => mTypeEventSystem.Send<TEvent>();

		public void SendEvent<TEvent>(TEvent e) => mTypeEventSystem.Send<TEvent>(e);

		// 注册和注销事件
		public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.Register<TEvent>(onEvent);

		public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.UnRegister<TEvent>(onEvent);
	}

	// 事件接口定义
	public interface IOnEvent<T>
	{
		void OnEvent(T e);
	}

	// 全局事件扩展方法
	public static class OnGlobalEventExtension
	{
		public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
			TypeEventSystem.Global.Register<T>(self.OnEvent);

		public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
			TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
	}

	#endregion

	#region Controller

	// 控制器接口
	public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel,
		ICanRegisterEvent, ICanSendQuery, ICanGetUtility
	{
	}

	#endregion

	#region System

	// 系统接口
	public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility,
		ICanRegisterEvent, ICanSendEvent, ICanGetSystem
	{
		void Init();
	}

	// 抽象系统类
	public abstract class AbstractSystem : ISystem
	{
		private IArchitecture mArchitecture;

		IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

		void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

		void ISystem.Init() => OnInit();

		protected abstract void OnInit();
	}

	#endregion

	#region Model

	// 模型接口
	public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanGetUtility, ICanSendEvent
	{
		void Init();// 初始化方法
	}

	// 抽象模型类
	public abstract class AbstractModel : IModel
	{
		private IArchitecture mArchitecturel;

		// 获取所属架构
		IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecturel;

		// 设置所属架构
		void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecturel = architecture;

		// 模型的初始化方法（由子类实现）
		void IModel.Init() => OnInit();

		protected abstract void OnInit();
	}

	#endregion

	#region Utility

	// 实用工具接口
	public interface IUtility
	{
		// 通常用于定义通用功能或服务
	}

	#endregion

	#region Command

	// 命令接口
	public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel, ICanGetUtility,
		ICanSendEvent, ICanSendCommand, ICanSendQuery
	{
		void Execute();// 执行命令的方法
	}

	// 带结果的命令接口
	public interface ICommand<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel,
		ICanGetUtility,
		ICanSendEvent, ICanSendCommand, ICanSendQuery
	{
		TResult Execute();// 执行命令并返回结果的方法
	}

	// 抽象命令类
	public abstract class AbstractCommand : ICommand
	{
		private IArchitecture mArchitecture;

		// 获取所属架构
		IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

		// 设置所属架构
		void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

		// 执行命令的方法（由子类实现）
		void ICommand.Execute() => OnExecute();

		protected abstract void OnExecute();
	}

	// 带结果的抽象命令类	
	public abstract class AbstractCommand<TResult> : ICommand<TResult>
	{
		private IArchitecture mArchitecture;

		// 获取所属架构
		IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

		// 设置所属架构
		void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

		// 执行命令并返回结果的方法（由子类实现）
		TResult ICommand<TResult>.Execute() => OnExecute();

		protected abstract TResult OnExecute();
	}

	#endregion

	#region Query

	// 查询接口
	public interface IQuery<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetSystem,
		ICanSendQuery
	{
		TResult Do();// 执行查询并返回结果的方法
	}

	// 抽象查询类
	public abstract class AbstractQuery<T> : IQuery<T>
	{
		public T Do() => OnDo();// 实际执行查询的方法

		protected abstract T OnDo();// 由子类实现的查询逻辑


		private IArchitecture mArchitecture;
		
		// 获取和设置所属架构
		public IArchitecture GetArchitecture() => mArchitecture;

		public void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
	}

	#endregion

	#region Rule
	// 定义归属于架构的接口
	public interface IBelongToArchitecture
	{
		IArchitecture GetArchitecture(); // 获取所属架构的方法
	}

	// 定义可以设置架构的接口
	public interface ICanSetArchitecture
	{
		void SetArchitecture(IArchitecture architecture); // 设置所属架构的方法
	}

	// 定义可以获取模型的接口
	public interface ICanGetModel : IBelongToArchitecture
	{
	}

	// 模型获取扩展方法
	public static class CanGetModelExtension
	{
		// 扩展方法，用于从架构中获取模型
		public static T GetModel<T>(this ICanGetModel self) where T : class, IModel =>
			self.GetArchitecture().GetModel<T>();
	}

	// 定义可以获取系统的接口
	public interface ICanGetSystem : IBelongToArchitecture
	{
	}

	// 系统获取扩展方法
	public static class CanGetSystemExtension
	{	
		// 扩展方法，用于从架构中获取系统
		public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
			self.GetArchitecture().GetSystem<T>();
	}

	// 定义可以获取实用程序的接口
	public interface ICanGetUtility : IBelongToArchitecture
	{
	}

	// 实用程序获取扩展方法
	public static class CanGetUtilityExtension
	{
		// 扩展方法，用于从架构中获取实用程序
		public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility =>
			self.GetArchitecture().GetUtility<T>();
	}

	// 定义可以注册事件的接口
	public interface ICanRegisterEvent : IBelongToArchitecture
	{
	}

	// 事件注册扩展方法
	public static class CanRegisterEventExtension
	{
		// 扩展方法，用于注册和注销事件
		public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
			self.GetArchitecture().RegisterEvent<T>(onEvent);

		public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
			self.GetArchitecture().UnRegisterEvent<T>(onEvent);
	}

	// 定义可以发送命令的接口
	public interface ICanSendCommand : IBelongToArchitecture
	{
	}

	// 命令发送扩展方法
	public static class CanSendCommandExtension
	{	
		// 扩展方法，用于发送命令
		// 发送无参数命令
		public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new() =>
			self.GetArchitecture().SendCommand<T>(new T());
		
		// 发送具体命令实例
		public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand =>
			self.GetArchitecture().SendCommand<T>(command);

		// 发送命令并获取结果
		public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
			self.GetArchitecture().SendCommand(command);
	}

	
	public interface ICanSendEvent : IBelongToArchitecture
	{
	}

	// 事件发送扩展方法
	public static class CanSendEventExtension
	{	
		// 发送无参数事件
		public static void SendEvent<T>(this ICanSendEvent self) where T : new() =>
			self.GetArchitecture().SendEvent<T>();
		
		// 发送具体事件实例
		public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetArchitecture().SendEvent<T>(e);
	}

	
	public interface ICanSendQuery : IBelongToArchitecture
	{
	}

	// 查询发送扩展方法
	public static class CanSendQueryExtension
	{	
		// 发送查询并获取结果
		public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query) =>
			self.GetArchitecture().SendQuery(query);
	}

	#endregion

	#region TypeEventSystem

	// 事件注销接口
	public interface IUnRegister
	{
		void UnRegister();
	}

	// 注销列表接口
	public interface IUnRegisterList
	{
		List<IUnRegister> UnregisterList { get; }
	}

	// 注销列表扩展方法
	public static class IUnRegisterListExtension
	{	
		// 将事件添加到注销列表
		public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList) =>
			unRegisterList.UnregisterList.Add(self);

		// 注销所有事件
		public static void UnRegisterAll(this IUnRegisterList self)
		{
			foreach (var unRegister in self.UnregisterList)
			{
				unRegister.UnRegister();
			}

			self.UnregisterList.Clear();
		}
	}

	// 自定义注销实现
	public struct CustomUnRegister : IUnRegister
	{
		private Action mOnUnRegister { get; set; }// 注销时的回调
		public CustomUnRegister(Action onUnRegister) => mOnUnRegister = onUnRegister;

		public void UnRegister()
		{
			mOnUnRegister.Invoke();
			mOnUnRegister = null;
		}
	}

	// UnRegisterOnDestroyTrigger 类用于 Unity 环境中自动注销事件
	#if UNITY_5_6_OR_NEWER
	// Unity 中的注销触发器
	public class UnRegisterOnDestroyTrigger : UnityEngine.MonoBehaviour
	{
		private readonly HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

		// 添加和移除需要在对象销毁时注销的事件
		public void AddUnRegister(IUnRegister unRegister) => mUnRegisters.Add(unRegister);

		public void RemoveUnRegister(IUnRegister unRegister) => mUnRegisters.Remove(unRegister);

		// Unity 生命周期中的销毁事件 用于自动注销
		private void OnDestroy()
		{
			foreach (var unRegister in mUnRegisters)
			{
				unRegister.UnRegister();
			}

			mUnRegisters.Clear();
		}
	}
	#endif

	// 扩展方法，用于 Unity GameObject 销毁时自动注销事件
	public static class UnRegisterExtension
	{
		#if UNITY_5_6_OR_NEWER
		public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, UnityEngine.GameObject gameObject)
		{
			var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

			if (!trigger)
			{
				trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
			}

			trigger.AddUnRegister(unRegister);

			return unRegister;
		}

		// 针对特定组件的同样操作
		public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
			where T : UnityEngine.Component =>
			self.UnRegisterWhenGameObjectDestroyed(component.gameObject);
		#endif
		
// Godot 环境中的类似实现		
#if GODOT
		public static IUnRegister UnRegisterWhenNodeExitTree(this IUnRegister unRegister, Godot.Node node)
		{
			node.TreeExiting += unRegister.UnRegister;
			return unRegister;
		}
#endif
	}

	// TypeEventSystem 类，用于事件的发送和管理
	public class TypeEventSystem
	{
		private readonly EasyEvents mEvents = new EasyEvents();

		// 全局事件系统实例
		public static readonly TypeEventSystem Global = new TypeEventSystem();

		// 发送事件
		public void Send<T>() where T : new() => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(new T());

		public void Send<T>(T e) => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(e);

		// 注册和注销事件
		public IUnRegister Register<T>(Action<T> onEvent) => mEvents.GetOrAddEvent<EasyEvent<T>>().Register(onEvent);

		public void UnRegister<T>(Action<T> onEvent)
		{
			var e = mEvents.GetEvent<EasyEvent<T>>();
			if (e != null)
			{
				e.UnRegister(onEvent);
			}
		}
	}

	#endregion

	#region IOC

	// IOCContainer 类，依赖注入容器的实现
	public class IOCContainer
	{
		private Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

		// 注册实例
		public void Register<T>(T instance)
		{
			var key = typeof(T);

			if (mInstances.ContainsKey(key))
			{
				mInstances[key] = instance;
			}
			else
			{
				mInstances.Add(key, instance);
			}
		}

		// 获取实例
		public T Get<T>() where T : class
		{
			var key = typeof(T);

			if (mInstances.TryGetValue(key, out var retInstance))
			{
				return retInstance as T;
			}

			return null;
		}
	}

	#endregion

	#region BindableProperty

	// 可绑定属性接口
	public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
	{
		new T Value { get; set; }// 属性值
		void SetValueWithoutEvent(T newValue);// 设置值但不触发事件
	}

	// 只读可绑定属性接口
	public interface IReadonlyBindableProperty<T> : IEasyEvent
	{
		T Value { get; }// 属性值

		// 注册带初始值的回调
		IUnRegister RegisterWithInitValue(Action<T> action);
		// 注销回调
		void UnRegister(Action<T> onValueChanged);
		// 注册回调
		IUnRegister Register(Action<T> onValueChanged);
	}

	// BindableProperty 类，实现可绑定属性
	public class BindableProperty<T> : IBindableProperty<T>
	{
		public BindableProperty(T defaultValue = default) => mValue = defaultValue;

		protected T mValue;// 属性的内部值
		
		// 比较器，用于判断值是否改变
		public static Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

		// 设置比较器
		public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)
		{
			Comparer = comparer;
			return this;
		}

		// Value 属性的实现
		public T Value
		{
			get => GetValue();
			set
			{
				if (value == null && mValue == null) return;
				if (value != null && Comparer(value, mValue)) return;

				SetValue(value);
				mOnValueChanged?.Invoke(value);
			}
		}

		// 设置和获取值的受保护方法
		protected virtual void SetValue(T newValue) => mValue = newValue;

		protected virtual T GetValue() => mValue;

		// 设置值但不触发事件
		public void SetValueWithoutEvent(T newValue) => mValue = newValue;

		private Action<T> mOnValueChanged = (v) => { };

		// 注册事件监听
		public IUnRegister Register(Action<T> onValueChanged)
		{
			mOnValueChanged += onValueChanged;
			return new BindablePropertyUnRegister<T>(this, onValueChanged);
		}

		// 注册带初始值的监听器
		public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
		{
			onValueChanged(mValue);
			return Register(onValueChanged);
		}

		// 注销监听器
		public void UnRegister(Action<T> onValueChanged) => mOnValueChanged -= onValueChanged;

		// 注册无返回值的通用事件
		IUnRegister IEasyEvent.Register(Action onEvent)
		{
			return Register(Action);
			void Action(T _) => onEvent();
		}
		
		public override string ToString() => Value.ToString();
	}

	// ComparerAutoRegister 类，用于 Unity 环境中的比较器自动注册
	internal class ComparerAutoRegister
	{
		#if UNITY_5_6_OR_NEWER
		[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void AutoRegister()
		{	
			// 为不同类型的可绑定属性设置默认比较器
			BindableProperty<int>.Comparer = (a, b) => a == b;
			BindableProperty<float>.Comparer = (a, b) => a == b;
			BindableProperty<double>.Comparer = (a, b) => a == b;
			BindableProperty<string>.Comparer = (a, b) => a == b;
			BindableProperty<long>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Vector2>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Vector3>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Vector4>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Color>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Color32>.Comparer = (a, b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
			BindableProperty<UnityEngine.Bounds>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Rect>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Quaternion>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Vector2Int>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.Vector3Int>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.BoundsInt>.Comparer = (a, b) => a == b;
			BindableProperty<UnityEngine.RangeInt>.Comparer = (a, b) => a.start == b.start && a.length == b.length;
			BindableProperty<UnityEngine.RectInt>.Comparer = (a, b) => a.Equals(b);
		}
		#endif
	}

	// BindablePropertyUnRegister 类，用于可绑定属性的注销
	public class BindablePropertyUnRegister<T> : IUnRegister
	{
		public BindablePropertyUnRegister(BindableProperty<T> bindableProperty, Action<T> onValueChanged)
		{
			BindableProperty = bindableProperty;
			OnValueChanged = onValueChanged;
		}

		public BindableProperty<T> BindableProperty { get; set; }

		public Action<T> OnValueChanged { get; set; }

		// 注销监听器
		public void UnRegister()
		{
			BindableProperty.UnRegister(OnValueChanged);
			BindableProperty = null;
			OnValueChanged = null;
		}
	}

	#endregion

	#region EasyEvent

	// 简单事件接口
	public interface IEasyEvent
	{
		IUnRegister Register(Action onEvent);// 注册事件监听器
	}

	// EasyEvent 类，用于无参数事件的管理
	public class EasyEvent : IEasyEvent
	{
		private Action mOnEvent = () => { };

		// 注册事件监听器
		public IUnRegister Register(Action onEvent)
		{
			mOnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}

		// 注销事件监听器
		public void UnRegister(Action onEvent) => mOnEvent -= onEvent;

		// 触发事件
		public void Trigger() => mOnEvent?.Invoke();
	}

	// EasyEvent 类，用于单参数事件的管理
	public class EasyEvent<T> : IEasyEvent
	{
		private Action<T> mOnEvent = e => { };

		// 注册事件监听器
		public IUnRegister Register(Action<T> onEvent)
		{
			mOnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}

		// 注销事件监听器
		public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;

		// 触发事件
		public void Trigger(T t) => mOnEvent?.Invoke(t);

		// 注册无返回值的通用事件
		IUnRegister IEasyEvent.Register(Action onEvent)
		{
			return Register(Action);
			void Action(T _) => onEvent();
		}
	}

	// EasyEvent 类，用于两个参数的事件	
	public class EasyEvent<T, K> : IEasyEvent
	{
		private Action<T, K> mOnEvent = (t, k) => { };

		// 注册事件监听器
		public IUnRegister Register(Action<T, K> onEvent)
		{
			mOnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}

		// 注销事件监听器
		public void UnRegister(Action<T, K> onEvent) => mOnEvent -= onEvent;

		// 触发事件
		public void Trigger(T t, K k) => mOnEvent?.Invoke(t, k);

		// 注册无返回值的通用事件
		IUnRegister IEasyEvent.Register(Action onEvent)
		{
			return Register(Action);
			void Action(T _, K __) => onEvent();
		}
	}

	// EasyEvent 类，用于三个参数的事件
	public class EasyEvent<T, K, S> : IEasyEvent
	{
		private Action<T, K, S> mOnEvent = (t, k, s) => { };

		// 注册事件监听器
		public IUnRegister Register(Action<T, K, S> onEvent)
		{
			mOnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}

		// 注销事件监听器
		public void UnRegister(Action<T, K, S> onEvent) => mOnEvent -= onEvent;

		// 触发事件
		public void Trigger(T t, K k, S s) => mOnEvent?.Invoke(t, k, s);

		// 注册无返回值的通用事件
		IUnRegister IEasyEvent.Register(Action onEvent)
		{
			return Register(Action);
			void Action(T _, K __, S ___) => onEvent();
		}
	}

	// EasyEvents 类，用于管理和检索事件
	public class EasyEvents
	{
		private static readonly EasyEvents mGlobalEvents = new EasyEvents();

		// 获取特定类型的事件
		public static T Get<T>() where T : IEasyEvent => mGlobalEvents.GetEvent<T>();

		// 注册新事件
		public static void Register<T>() where T : IEasyEvent, new() => mGlobalEvents.AddEvent<T>();

		private readonly Dictionary<Type, IEasyEvent> mTypeEvents = new Dictionary<Type, IEasyEvent>();

		// 添加事件到事件字典
		public void AddEvent<T>() where T : IEasyEvent, new() => mTypeEvents.Add(typeof(T), new T());

		// 获取事件实例
		public T GetEvent<T>() where T : IEasyEvent
		{
			return mTypeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
		}

		// 获取或添加事件实例
		public T GetOrAddEvent<T>() where T : IEasyEvent, new()
		{
			var eType = typeof(T);
			if (mTypeEvents.TryGetValue(eType, out var e))
			{
				return (T)e;
			}

			var t = new T();
			mTypeEvents.Add(eType, t);
			return t;
		}
	}

	#endregion


	#region Event Extension

	// OrEvent 类，用于组合多个事件
	public class OrEvent : IUnRegisterList
	{
		public OrEvent Or(IEasyEvent easyEvent)
		{
			easyEvent.Register(Trigger).AddToUnregisterList(this);
			return this;
		}

		private Action mOnEvent = () => { };

		// 注册和注销事件监听器
		public IUnRegister Register(Action onEvent)
		{
			mOnEvent += onEvent;
			return new CustomUnRegister(() => { UnRegister(onEvent); });
		}

		public void UnRegister(Action onEvent)
		{
			mOnEvent -= onEvent;
			this.UnRegisterAll();
		}

		private void Trigger() => mOnEvent?.Invoke();

		public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
	}

	// OrEvent 扩展方法	
	public static class OrEventExtensions
	{	
		// 扩展方法，用于将多个事件组合在一起
		public static OrEvent Or(this IEasyEvent self, IEasyEvent e) => new OrEvent().Or(self).Or(e);
	}

	#endregion

#if UNITY_EDITOR
	// Unity 编辑器中的菜单项
	internal class EditorMenus
	{	
		// 在 Unity 编辑器中添加菜单项，用于安装 QFrameworkWithToolKits
		[UnityEditor.MenuItem("QFramework/Install QFrameworkWithToolKits")]
		public static void InstallPackageKit() => UnityEngine.Application.OpenURL("https://qframework.cn/qf");
	}
#endif
}