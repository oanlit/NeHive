using LibVLCSharp.Shared;
using NeHive.Reactive;
using NeHive.UI.Avalonia;
using Avalonia.Threading;
using static NeHive.UI.Avalonia.Components.BaseComponent;
using static NeHive.UI.Avalonia.Components.ControlFlow;

namespace NeHive.Sample.Avalonia;

public static class MusicPlayerDemo
{
    // private static readonly MutSignal<string?> LibVlcPath = new("D:/Software/VideoLAN/VLC");
    private static readonly MutSignal<string?> LibVlcPath = new("D:/Software/VideoLAN/VLC/");
    private static Scope _scope = new();

    static MusicPlayerDemo()
    {
        _scope.CreateEffect(() =>
        {
            var path = LibVlcPath.RxValue;
            if (path is null) return;
            Console.WriteLine($"选择的路径: {path}");
            Core.Initialize(path);
        });
    }

    private static IElement CorePlayerComp(UiScope uiScope)
    {
        // ---------- 播放引擎 ----------
        var libVlc = new LibVLC();
        var mediaPlayer = new MediaPlayer(libVlc);

        // ---------- 状态 ----------
        var playlist = new MutSignal<IReadOnlyList<TrackInfo>>([]);
        var currentIndex = new MutSignal<int>(-1);
        var isPlaying = new MutSignal<bool>(false);
        var volume = new MutSignal<int>(50);
        var position = new MutSignal<TimeSpan>(TimeSpan.Zero);
        var duration = new MutSignal<TimeSpan>(TimeSpan.FromSeconds(1));
        var selectedPath = new MutSignal<string?>(null);
        var trackName = new MutSignal<string>("未选择曲目");

        // ---------- UI ----------
        var rootElement = uiScope.RootElement(new(strStyle: "m-6 gap-5 flex-col bg-gray-50 rounded-2xl p-6")
        {
            HTextBlock("🎵 NeHive Music Player", strStyle: "text-xl font-bold fg-sky-800 mb-2"),

            // 播放控制区
            HStackPanel(new(strStyle: "gap-6 flex-row items-center justify-between bg-white rounded-xl p-4 shadow-sm")
            {
                // 专辑封面 + 曲目信息
                HStackPanel(new(strStyle: "gap-4 flex-row items-center")
                {
                    HButton(strStyle: "w-20 h-20 bg-gray-200 rounded-lg"),
                    HStackPanel(new(strStyle: "gap-1 flex-col")
                    {
                        HTextBlock(trackName, strStyle: "text-base font-semibold fg-gray-800"),
                        HTextBlock("未知艺术家", strStyle: "text-sm fg-gray-500")
                    }) // HStackPanel
                }), // HStackPanel

                // 中间：按钮 + 进度
                HStackPanel(new(strStyle: "gap-3 flex-col flex-1")
                {
                    // 播放按钮组
                    HStackPanel(new(strStyle: "gap-3 flex-row justify-center")
                    {
                        HButton("⏮",
                            strStyle: "px-3 py-1 bg-gray-200 hover:bg-gray-300 rounded-lg text-lg",
                            onClick: _ => PlayLast()
                        ),
                        HButton(new(() => isPlaying.RxValue ? "⏸" : "▶️"),
                            strStyle:
                            "px-4 py-2 bg-sky-500 hover:bg-sky-600 click:bg-sky-700 fg-white rounded-full text-lg",
                            onClick: _ => isPlaying.RxValue = !isPlaying.Value
                        ), // HButton
                        HButton("⏭",
                            strStyle: "px-3 py-1 bg-gray-200 hover:bg-gray-300 rounded-lg text-lg",
                            onClick: _ => PlayNext()
                        ) // HButton
                    }), // HStackPanel

                    // 进度条
                    HStackPanel(new(strStyle: "gap-2 flex-row items-center")
                    {
                        HTextBlock(
                            new(() => position.RxValue.ToString(@"mm\:ss")),
                            strStyle: "text-xs fg-gray-500 w-12 text-right"
                        ), // HTextBlock
                        HSlider(
                            value: new(() => position.RxValue.TotalMilliseconds),
                            minimum: 0,
                            maximum: new(() => duration.RxValue.TotalMilliseconds),
                            strStyle: "w-64 flex-row",
                            onValueChanged: val =>
                            {
                                if (Math.Abs(val - position.Value.TotalMilliseconds) > 1000)
                                    mediaPlayer.SeekTo(TimeSpan.FromMilliseconds(val));
                            }
                        ), // HSlider
                        HTextBlock(
                            new(() => duration.RxValue.ToString(@"mm\:ss")),
                            strStyle: "text-xs fg-gray-500 w-12"
                        ) // HTextBlock
                    }) // HStackPanel
                }), // HStackPanel

                // 音量
                HStackPanel(new(strStyle: "gap-2 flex-row items-center")
                {
                    HTextBlock("🔊", strStyle: "text-base"),
                    HSlider(
                        value: new Computed<double>(() => volume.RxValue),
                        minimum: 0,
                        maximum: 100,
                        strStyle: "w-24",
                        onValueChanged: val =>
                        {
                            var v = (int)val;
                            volume.RxValue = v;
                            mediaPlayer.Volume = v;
                        }
                    ), // HSlider
                    HTextBlock(new(() => $"{volume.RxValue}%"), strStyle: "text-xs fg-gray-500 w-10")
                }) // HStackPanel
            }), // HStackPanel

            // 播放列表操作
            HStackPanel(new(strStyle: "gap-3 flex-col")
            {
                HStackPanel(new(strStyle: "gap-3 flex-row")
                {
                    HFilePicker(
                        bindSelectedPath: selectedPath,
                        title: "添加音频文件",
                        filters:
                        [
                            new FilePickerFilter("音频文件", "*.mp3", "*.wav", "*.flac"),
                            new FilePickerFilter("所有文件", "*.*")
                        ]
                    ), // HFilePicker
                    HButton("清空列表",
                        strStyle: "px-3 py-1 bg-rose-400 hover:bg-rose-500 fg-white rounded-lg",
                        onClick: _ =>
                        {
                            mediaPlayer.Stop();
                            playlist.RxValue = [];
                            Console.WriteLine("HButton(清空列表) 执行了 currentIndex 的修改");
                            currentIndex.RxValue = -1;
                        }
                    ) // HButton
                }), // HStackPanel

                // 列表
                HScrollViewer(new(strStyle: "h-48 bg-white rounded-lg border border-gray-200 p-2")
                {
                    ForEach<TrackInfo>(new(playlist)
                    {
                        Container = HStackPanel(new(strStyle: "gap-1 flex-col")),
                        ItemTemplate = (track, index) =>
                            HButton(track.Title,
                                strStyle: new(() =>
                                {
                                    var isActive = index.RxValue == currentIndex.RxValue;
                                    return
                                        $"px-3 py-2 text-left rounded-lg {(isActive ? "bg-sky-100 fg-sky-800 font-semibold" : "bg-transparent fg-gray-700 hover:bg-gray-100")}";
                                }),
                                onClick: _ =>
                                {
                                    Console.WriteLine("HButton(点击曲目) 执行了 currentIndex 的修改");
                                    currentIndex.RxValue = index.Value;
                                    PlayTrack(track);
                                }
                            ) // HButton
                        // ForEach<TrackInfo>.ItemTemplate
                    }) // ForEach<TrackInfo>
                }) // HScrollViewer
            }) // HStackPanel
        }); // rootElement

        // 引擎事件 → 信号
        mediaPlayer.Playing += (_, _) => isPlaying.PostToUi(true);
        mediaPlayer.Paused += (_, _) => isPlaying.PostToUi(false);
        mediaPlayer.Stopped += (_, _) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                isPlaying.RxValue = false;
                position.RxValue = TimeSpan.Zero;
            });
        };
        mediaPlayer.TimeChanged += (_, e) => position.PostToUi(TimeSpan.FromMilliseconds(e.Time));
        mediaPlayer.LengthChanged += (_, e) => duration.PostToUi(TimeSpan.FromMilliseconds(e.Length));
        mediaPlayer.EndReached += (_, _) =>
        {
            Console.WriteLine("mediaPlayer.EndReached 事件触发");
            PlayNext();
        };

        uiScope.CreateEffect(epochScope =>
        {
            var path = epochScope.Pull(selectedPath);
            if (string.IsNullOrEmpty(path)) return;

            var list = playlist.Value.ToList();
            list.Add(new TrackInfo(Path.GetFileNameWithoutExtension(path), path));
            playlist.RxValue = list;
        });

        uiScope.CreateEffect(epochScope =>
        {
            var isPlayingValue = epochScope.Pull(isPlaying);

            if (!isPlayingValue) mediaPlayer.Pause();
            else
            {
                var list = playlist.Value;
                if (list.Count > 0 && currentIndex.Value < 0)
                {
                    Console.WriteLine("CreateEffect(isPlayingValue) 执行了 currentIndex 的修改");
                    currentIndex.RxValue = 0;
                    PlayTrack(list[0]);
                }
                else mediaPlayer.Play();
            }
        });


        uiScope.OnDispose += () =>
        {
            mediaPlayer.Dispose();
            libVlc.Dispose();
        };

        return rootElement;

        void PlayLast()
        {
            Dispatcher.UIThread.Post(() =>
            {
                position.RxValue = TimeSpan.Zero;
                var list = playlist.Value;
                if (list.Count == 0) return;
                var idx = currentIndex.Value <= 0 ? list.Count - 1 : currentIndex.Value - 1;
                Console.WriteLine("PlayLast 执行了 currentIndex 的修改");
                currentIndex.RxValue = idx;
                PlayTrack(list[idx]);
            });
        }

        void PlayNext()
        {
            Dispatcher.UIThread.Post(() =>
            {
                position.RxValue = TimeSpan.Zero;
                var list = playlist.Value;
                if (list.Count == 0) return;
                var idx = (currentIndex.Value + 1) % list.Count;
                Console.WriteLine("PlayNext 执行了 currentIndex 的修改");
                currentIndex.RxValue = idx;
                PlayTrack(list[idx]);
            });
        }

        void PlayTrack(TrackInfo track)
        {
            Dispatcher.UIThread.Post(() =>
            {
                mediaPlayer.Play(new Media(libVlc, track.FilePath));
                trackName.RxValue = track.Title;
            });
        }
    }

    public static IElement CorePlayer() => Element.WithScope(CorePlayerComp);

    public static IElement MusicPlayer()
    {
        var asyncMemo = _scope.CreateAsyncMemo(async () =>
        {
            await Task.Delay(1);
            return true;
        });
        return Show(new(new(() => LibVlcPath.RxValue is not null))
        {
            IfTrue = () => Loading<bool>(new(asyncMemo)
            {
                Success = _ => CorePlayer()
            }),
            IfFalse = () => HStackPanel(new()
            {
                HTextBlock("请选择LibVLC路径"),
                HFolderPicker(
                    bindSelectedPath: LibVlcPath,
                    title: "LibVLC路径"
                ) // HFilePicker
            })
        });
    }

    private record TrackInfo(string Title, string FilePath);
}

public static class ThreadSafeExtensions
{
    public static void PostToUi<T>(this MutSignal<T> signal, T value)
    {
        Dispatcher.UIThread.Post(() => signal.RxValue = value);
    }
}