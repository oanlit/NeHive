using LibVLCSharp.Shared;
using NeHive.Model;
using NeHive.Reactive;
using NeHive.UI.Avalonia;
using Avalonia.Threading;
using Avalonia.Media;
using static NeHive.UI.Avalonia.Components.BaseComponent;
using static NeHive.UI.Avalonia.Components.ControlFlow;

namespace NeHive.Sample.Avalonia;

public static class MediaPlayerExtensions
{
    extension(Scope scope)
    {
        public (LibVLC, MediaPlayer) CreateMediaPlayer()
        {
            var libVlc = new LibVLC();
            var mediaPlayer = new MediaPlayer(libVlc);

            scope.OnCleanup += () =>
            {
                mediaPlayer.Dispose();
                libVlc.Dispose();
            };

            return (libVlc, mediaPlayer);
        }
    }
}

public static class MusicPlayerDemo
{
    private static readonly MutSignal<string?> LibVlcPath = new("D:/Software/VideoLAN/VLC");

    // private static readonly MutSignal<string?> LibVlcPath = new(null);
    private static Scope _scope = new();

    public static readonly ContextKey<string> Theme = new();

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
        var (libVlc, mediaPlayer) = uiScope.CreateMediaPlayer();

        // ---------- 状态 ----------

        var playlist = new MutSignal<IReadOnlyList<TrackInfo>>([]);
        var currentIndex = new MutSignal<int>(-1,
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));
        var isPlaying = new MutSignal<bool>(false,
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));
        var volume = new MutSignal<int>(50);
        var position = new MutSignal<TimeSpan>(TimeSpan.Zero,
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));
        var duration = new MutSignal<TimeSpan>(TimeSpan.FromSeconds(1),
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));
        var selectedPath = new MutSignal<string?>(null);

        var currentMedia = new MutSignal<Media?>(null,
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));

        var triggerAsyncMemo = new MutSignal<bool>(false,
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));
        var songInfo = uiScope.CreateAsyncMemo<SongInfo?>(async epochScope =>
        {
            _ = epochScope.Pull(triggerAsyncMemo);
            var indexValue = epochScope.Pull(currentIndex);
            if (indexValue < 0) return null;

            var list = playlist.Value;
            var media = new Media(libVlc, list[indexValue].FilePath);

            await media.Parse(timeout: 5000);

            var title = media.Meta(MetadataType.Title) ?? "未知标题";
            var artist = media.Meta(MetadataType.Artist) ?? "未知歌手";
            var album = media.Meta(MetadataType.Album) ?? "未知专辑";

            var coverPath = media.Meta(MetadataType.ArtworkURL);
            if (coverPath is not null) coverPath = new Uri(coverPath).LocalPath;

            currentMedia.RxValue = media;
            return new SongInfo(title, artist, album, coverPath);
        });

        // ---------- UI ----------
        var rootElement = uiScope.RootElement(new(strStyle: "m-6 w-full gap-5 vertical bg-gray-50 rounded-2xl p-6")
        {
            HTextBlock("🎵 NeHive Music Player", strStyle: "text-xl font-bold fg-sky-800 mb-2"),

            // 播放控制区
            HStackPanel(new(strStyle: "p-4 horizontal items-center justify-between bg-white rounded-xl shadow-sm")
            {
                Loading<SongInfo?>(new(songInfo)
                {
                    Success = user => HContext(Theme, "light", () => Audio(user)),
                    Loading = Audio,
                    Error = _ => Audio(null),
                }) // Loading<SongInfo?>
            }), // HStackPanel

            // 播放列表操作
            HStackPanel(new(strStyle: "w-full gap-3 vertical")
            {
                HStackPanel(new(strStyle: "gap-3 horizontal")
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
                            currentIndex.RxValue = -1;
                        }
                    ) // HButton
                }), // HStackPanel

                // 列表
                HScrollViewer(new(strStyle: "w-full h-48 bg-white rounded-lg border border-gray-200 p-2")
                {
                    ForEach<TrackInfo>(new(playlist)
                    {
                        Container = HStackPanel(new(strStyle: "gap-1 vertical")),
                        ItemTemplate = (track, index) =>
                            HButton(track.Title,
                                strStyle: new(() =>
                                {
                                    var isActive = index.RxValue == currentIndex.RxValue;
                                    return
                                        $"px-3 py-2 text-left rounded-lg {(isActive ? "bg-sky-100 fg-sky-800 font-semibold" : "bg-transparent fg-gray-700 hover:bg-gray-100")}";
                                }),
                                onClick: _ => currentIndex.RxValue = index.Value
                            ) // HButton
                        // ForEach<TrackInfo>.ItemTemplate
                    }) // ForEach<TrackInfo>
                }) // HScrollViewer
            }), // HStackPanel

            // 中间：按钮 + 进度
            HStackPanel(new(strStyle: "gap-3 vertical center")
            {
                // 播放按钮组
                HStackPanel(new(strStyle: "gap-5 horizontal center")
                {
                    HContentButton(new(
                        strStyle: "my-auto w-4 h-4 hover:opacity-50",
                        onClick: _ => PlayLast())
                    {
                        HSvgImage("~/Assets/skip-back.svg",
                            strStyle: "w-4 h-4 font-extralight fg-black bg-black/0"),
                    }), // HContentButton
                    HContentButton(new(strStyle:
                        """
                        w-11 h-11 center fg-white 
                        bg-gradient-br bg-from-blue-200 bg-to-violet-800
                        opacity-80 hover:opacity-90 click:opacity-100
                        rounded-full
                        """,
                        onClick: _ => isPlaying.RxValue = !isPlaying.Value)
                    {
                        Show(new(isPlaying)
                        {
                            IfFalse = () => HSvgImage("~/Assets/play.svg",
                                strStyle: """
                                          ml-1 w-6 h-6 font-normal
                                          fg-gradient-r fg-from-blue-400 fg-to-red-800
                                          """
                            ),
                            // IfFalse
                            IfTrue = () => HSvgImage("~/Assets/stretch-vertical.svg",
                                strStyle: """
                                          ml-0.5 w-6 h-6 font-extralight
                                          fg-gradient-r fg-from-blue-400 fg-to-red-800
                                          """
                            ) // IfTrue
                        }) // Show
                    }), // HContentButton
                    HContentButton(new(
                        strStyle: "center bg-black/0 hover:opacity-50",
                        onClick: _ => PlayNext())
                    {
                        HSvgImage("~/Assets/skip-forward.svg",
                            strStyle: "w-4 h-4 font-extralight fg-black bg-black/0")
                    }) // HContentButton
                }), // HStackPanel

                // 进度条
                HStackPanel(new(strStyle: "gap-2 horizontal items-center")
                {
                    HTextBlock(
                        new(() => position.RxValue.ToString(@"mm\:ss")),
                        strStyle: "text-xs fg-gray-500 w-12 text-right"
                    ), // HTextBlock
                    HCustomSlider(new(value: new(() => position.RxValue.TotalMilliseconds),
                        minimum: 0,
                        maximum: new(() => duration.RxValue.TotalMilliseconds),
                        strStyle: "w-64 h-0.5 horizontal fg-blue-200 bg-gray-500",
                        onValueChanged: val =>
                        {
                            if (Math.Abs(val - position.Value.TotalMilliseconds) > 1000)
                                mediaPlayer.SeekTo(TimeSpan.FromMilliseconds(val));
                        })
                    {
                        Thumb = state => Show(new(new(() => state.IsHover.RxValue || state.IsDragging.RxValue))
                        {
                            IfTrue = () => HSvgImage("~/Assets/circle-star.svg",
                                strStyle: "w-4 h-4 font-extralight fg-yellow-500 bg-yellow-200 rounded-full")
                        }) // HCustomSlider.Thumb
                    }), // HCustomSlider
                    HTextBlock(
                        new(() => duration.RxValue.ToString(@"mm\:ss")),
                        strStyle: "text-xs fg-gray-500 w-12"
                    ) // HTextBlock
                }) // HStackPanel
            }), // HStackPanel

            // 音量
            HStackPanel(new(strStyle: "gap-x-2 horizontal")
            {
                Match<int>(new(volume)
                {
                    Cases = new()
                    {
                        [v => v == 0] = () =>
                            HSvgImage("~/Assets/volume.svg", strStyle: "my-auto w-4 h-4 font-extralight"),
                        [v => v < 75] = () =>
                            HSvgImage("~/Assets/volume-1.svg", strStyle: "my-auto w-4 h-4 font-extralight")
                    },
                    Default = () => HSvgImage("~/Assets/volume-2.svg", strStyle: "my-auto w-4 h-4 font-extralight")
                }),
                HSlider(value: new(() => volume.RxValue),
                    minimum: 0,
                    maximum: 100,
                    strStyle: "my-auto w-24",
                    onValueChanged: val =>
                    {
                        var v = (int)val;
                        volume.RxValue = v;
                        mediaPlayer.Volume = v;
                    }), // HSlider
                HTextBlock(new(() => $"{volume.RxValue}%"), strStyle: "text-xs fg-gray-500 w-10")
            }) // HStackPanel
        }); // rootElement

        // 引擎事件 → 信号
        mediaPlayer.Playing += (_, _) => isPlaying.RxValue = true;
        mediaPlayer.Paused += (_, _) => isPlaying.RxValue = false;
        mediaPlayer.Stopped += (_, _) =>
        {
            isPlaying.RxValue = false;
            position.RxValue = TimeSpan.Zero;
        };
        mediaPlayer.TimeChanged += (_, e) => position.RxValue = TimeSpan.FromMilliseconds(e.Time);
        mediaPlayer.LengthChanged += (_, e) => duration.RxValue = TimeSpan.FromMilliseconds(e.Length);
        mediaPlayer.EndReached += (_, _) => PlayNext();

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
                    currentIndex.RxValue = 0;

                else mediaPlayer.Play();
            }
        });

        uiScope.CreateEffect(epochScope =>
        {
            var media = epochScope.Pull(currentMedia);
            if (media is null) return;

            mediaPlayer.Play(media);
        });

        return rootElement;

        void PlayLast()
        {
            position.RxValue = TimeSpan.Zero;
            var list = playlist.Value;
            if (list.Count == 0) return;
            if (list.Count == 1)
            {
                mediaPlayer.SeekTo(TimeSpan.FromMilliseconds(0));
                triggerAsyncMemo.RxValue = !triggerAsyncMemo.Value;
                return;
            }

            var idx = currentIndex.Value <= 0 ? list.Count - 1 : currentIndex.Value - 1;
            currentIndex.RxValue = idx;
        }

        void PlayNext()
        {
            position.RxValue = TimeSpan.Zero;

            var list = playlist.Value;
            if (list.Count == 0) return;
            if (list.Count == 1)
            {
                // mediaPlayer.SeekTo(TimeSpan.FromMilliseconds(0));
                triggerAsyncMemo.RxValue = !triggerAsyncMemo.Value;
                return;
            }

            var idx = (currentIndex.Value + 1) % list.Count;
            currentIndex.RxValue = idx;
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
                Success = _ => CorePlayer(),
                Error = ex =>
                {
                    Console.WriteLine(ex.StackTrace);
                    return HTextBlock(new($"RxError: {ex.Message}"));
                }
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

    // 专辑封面 + 曲目信息
    private static IElement Audio(SongInfo? song)
    {
        return HStackPanel(new(strStyle: "w-110 gap-4 horizontal items-center")
        {
            Show(new(song?.CoverPath is not null)
            {
                IfFalse = () => HButton(strStyle: "w-32 h-32 bg-gray-200 rounded-xl"),
                IfTrue = () => HUriImage(song?.CoverPath,
                    stretch: Stretch.UniformToFill,
                    strStyle: "w-32 h-32 rounded-xl transition-transform duration-200 hover:scale-110")
            }),

            HStackPanel(new(strStyle: "gap-1 vertical")
            {
                HTextBlock(song?.Title ?? "未知标题", strStyle: "max-w-100 text-base font-semibold fg-gray-800"),
                HTextBlock(song?.Artist ?? "未知歌手", strStyle: "max-w-75 text-sm fg-gray-500"),
                HTextBlock(song?.Album ?? "未知专辑", strStyle: "max-w-50 text-xs fg-gray-500")
            }) // HStackPanel
        }); // HStackPanel
    }

    private record TrackInfo(string Title, string FilePath);

    private record SongInfo(string Title, string Artist, string Album, string? CoverPath);
}