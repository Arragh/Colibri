1) Разузнать побольше про Backpressure, то есть чтобы прокси не читал быстрее, чем может записать
2) Продумать реализацию избавления от hop-by-hop заголовков
3) Пошерстить возможность настройки самого Kestrel прямо в Program.cs, типа builder.WebHost.ConfigureKestrel
4) Рассмотреть возможность использовать ObjectPool<T> для PipelineContext.
