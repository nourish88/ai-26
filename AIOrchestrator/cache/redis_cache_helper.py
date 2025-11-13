# cache_helper.py
from __future__ import annotations
import json
from typing import Any,  Optional
from server import ConfigurationManager

from fastapi_cache import FastAPICache
from fastapi_cache.backends.redis import RedisBackend
from redis import asyncio as aioredis
from .models import UserCacheItem



class RedisCache:
    url:str
    password:Optional[str] = None
    redis:Optional[aioredis.Redis] = None
    cache: Optional[FastAPICache] = None
    default_cache_expire:int
    def __init__(self):
        self.redis_url = ConfigurationManager.get_redis_url()
        self.password = ConfigurationManager.get_redis_password()
        self.redis: Optional[aioredis.Redis] = None
        self.cache: Optional[FastAPICache] = None
        self.default_cache_expire = ConfigurationManager.get_default_cache_expire()

    async def init(self) -> None:
        self.redis = await aioredis.from_url(
            self.redis_url,
            encoding="utf-8",
            decode_responses=True,
            password = self.password
        )
        fast_api_cache = FastAPICache()
        fast_api_cache.init(RedisBackend(self.redis))
        self.cache = fast_api_cache

    async def close(self) -> None:
        if self.redis:
            await self.redis.close()

    async def get(self, key: str) -> Any | None:
        if not self.cache:
            raise RuntimeError("Cache not initialized")
        value = await self.cache.get_backend().get(key)
        return json.loads(value) if value else None
    
    async def get_user_cache_item(self,user_id : str)->UserCacheItem|None:
        cache_value = await self.get(user_id)
        if not cache_value:
            return None
        return UserCacheItem(**cache_value)

    async def set(self, key: str, value: Any, expire: int | None = None) -> None:
        if not self.cache:
            raise RuntimeError("Cache not initialized")
        exp=expire
        if not expire:
            exp=self.default_cache_expire
        bytes = self.cache.get_coder().encode(value=value)
        await self.cache.get_backend().set(key, bytes, expire=exp)

    async def delete(self, key: str) -> None:
        if not self.cache:
            raise RuntimeError("Cache not initialized")
        await self.cache.get_backend().clear(key=key)

    async def clear(self) -> None:
        if not self.cache:
            raise RuntimeError("Cache not initialized")
        await self.cache.clear()

CacheHelper = RedisCache()
async def init_cache_helper()->None:
    await CacheHelper.init()
async def close_cache_helper()->None:
    await CacheHelper.close()


