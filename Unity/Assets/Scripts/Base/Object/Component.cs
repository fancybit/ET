﻿using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
	[BsonIgnoreExtraElements]
	public abstract partial class Component : Object, IDisposable
	{
		[BsonIgnore]
		public long InstanceId { get; protected set; }

		[BsonIgnore]
		private bool isFromPool;

		[BsonIgnore]
		public bool IsFromPool
		{
			get
			{
				return this.isFromPool;
			}
			set
			{
				this.isFromPool = value;

				if (this.InstanceId == 0)
				{
					this.InstanceId = IdGenerater.GenerateId();
					Game.EventSystem.Add(this);
				}
			}
		}

		[BsonIgnore]
		public bool IsDisposed
		{
			get
			{
				return this.InstanceId == 0;
			}
		}


		[BsonIgnoreIfDefault]
		[BsonDefaultValue(0L)]
		[BsonElement]
		[BsonId]
		public long Id { get; set; }

		[BsonIgnore]
		public Component Parent { get; set; }

		public T GetParent<T>() where T : Component
		{
			return this.Parent as T;
		}

		[BsonIgnore]
		public Entity Entity
		{
			get
			{
				return this.Parent as Entity;
			}
		}

		protected Component()
		{
			this.InstanceId = IdGenerater.GenerateId();
			Game.EventSystem.Add(this);
			this.Id = this.InstanceId;
		}

		protected Component(long instanceId)
		{
			this.InstanceId = instanceId;
			Game.EventSystem.Add(this);
		}

		public virtual void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			Game.EventSystem.Remove(this.InstanceId);

			this.InstanceId = 0;

			if (this.IsFromPool)
			{
				Game.ObjectPool.Recycle(this);
			}

			// 触发Desdroy事件
			Game.EventSystem.Desdroy(this);
		}
	}
}