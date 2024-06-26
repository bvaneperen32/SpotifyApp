﻿using SpotifyAPI.Web;
using SpotifyApp.Models;
using ColorThiefDotNet;
using System.Drawing;
using System.Linq; 

namespace SpotifyApp.Services
{
    public class SpotifyService
    {
        private readonly SpotifyClient _spotify;

        public SpotifyService(PKCETokenResponse tokenResponse)
        {
            var authenticator = new PKCEAuthenticator("YourClientId", tokenResponse);
            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);
            _spotify = new SpotifyClient(config);
        }

        public async Task<TopArtistsViewModel> GetTopArtists()
        {
            var topArtists = await _spotify.Personalization.GetTopArtists();

            var artistsInfos = topArtists.Items.Select(artist => new ArtistInfo
            {
                Name = artist.Name,
                ImageUrl = artist.Images.FirstOrDefault()?.Url,
                Popularity = artist.Popularity,
                Genres = artist.Genres,
                Followers = artist.Followers.Total,
                SpotifyUrl = artist.ExternalUrls["spotify"]
            }).ToList();

            foreach (var artistInfo in artistsInfos)
            {
                artistInfo.DominantColors = GetDominantColors(artistInfo.ImageUrl).Result; 
            }

            return new TopArtistsViewModel
            {
                Artists = artistsInfos
            };
        }

        public async Task<TopTracksViewModel> GetTopTracks()
        {
            var topTracks = await _spotify.Personalization.GetTopTracks();

            var tracks = topTracks.Items.Select(track => new Track
            {
                Name = track.Name,
                Artist = track.Artists.FirstOrDefault()?.Name,
                Album = track.Album.Name,
                Popularity = track.Popularity,
                ImageUrl = track.Album.Images.FirstOrDefault()?.Url,
                SongUrl = track.ExternalUrls["spotify"]
            }).ToList();

            foreach (var track in tracks)
            {
                track.DominantColors = GetDominantColors(track.ImageUrl).Result;
            }

            return new TopTracksViewModel
            {
                Tracks = tracks
            };
        }

        public async Task<PlaylistsViewModel> GetPlaylists()
        {
            var playlists = await _spotify.Playlists.CurrentUsers();

            var playlistsInfos = playlists.Items.Select(playlist => new Playlist
            {
                Name = playlist.Name,
                ImageUrl = playlist.Images.FirstOrDefault()?.Url,
                SpotifyUrl = playlist.ExternalUrls["spotify"],
                Id = playlist.Id
            }).ToList();

            return new PlaylistsViewModel
            {
                Playlists = playlistsInfos
            };
        }

        public async Task<RecTracksViewModel> GetRecommendations()
        {
            var topTracksRequest = new PersonalizationTopRequest
            {
                Limit = 5,
                TimeRangeParam = PersonalizationTopRequest.TimeRange.ShortTerm
            };

            var topTracks = await _spotify.Personalization.GetTopTracks(topTracksRequest);

            var trackIds = topTracks.Items.Select(track => track.Id).ToList();

            if (!trackIds.Any())
            {
                return new RecTracksViewModel
                {
                    RecTracks = new List<RecTrack>()
                };
            }


            var recommendationsRequest = new RecommendationsRequest
            {
                Limit = 20 
            };

            foreach (var trackId in trackIds.Take(5))
            {
                recommendationsRequest.SeedTracks.Add(trackId);
            }

            var recommendations = await _spotify.Browse.GetRecommendations(recommendationsRequest);
            var fullTracks = await Task.WhenAll(recommendations.Tracks.Select(async track => await _spotify.Tracks.Get(track.Id)));
            var recommendedTracks = fullTracks.Select(track => new RecTrack
            {
                TrackName = track.Name,
                ArtistName = track.Artists.FirstOrDefault()?.Name,
                AlbumName = track.Album.Name,
                TrackImage = track.Album.Images.FirstOrDefault()?.Url,
                TrackUrl = track.ExternalUrls["spotify"]
            }).ToList();

            return new RecTracksViewModel
            {
                RecTracks = recommendedTracks
            };

        }

        public async Task<List<string>> GetDominantColors(string imageUrl, int topColors = 3)
        {
            using (var client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(imageUrl);
                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var image = new Bitmap(ms))
                    {
                        var colorFrequency = new Dictionary<System.Drawing.Color, int>();

                        for (int x = 0; x < image.Width; x+= 30)
                        {
                            for (int y = 0; y < image.Height; y+= 30)
                            {
                                System.Drawing.Color originalColor = image.GetPixel(x, y);
                                // Simplify the color by rounding its RGB components
                                int r = RoundToNearest(originalColor.R, 25);
                                int g = RoundToNearest(originalColor.G, 25);
                                int b = RoundToNearest(originalColor.B, 25);
                                System.Drawing.Color simplifiedColor = System.Drawing.Color.FromArgb(r, g, b);

                                if (colorFrequency.ContainsKey(simplifiedColor))
                                {
                                    colorFrequency[simplifiedColor]++;
                                }
                                else
                                {
                                    colorFrequency[simplifiedColor] = 1;
                                }
                            }
                        }

                        // Get the top 'topColors' colors
                        var dominantColors = colorFrequency.OrderByDescending(kvp => kvp.Value)
                                                            .Take(topColors)
                                                            .Select(kvp => $"rgb({kvp.Key.R}, {kvp.Key.G}, {kvp.Key.B})")
                                                            .ToList();

                        return dominantColors;
                    }
                }
            }
        }

        // Helper method to round color components
        private int RoundToNearest(int component, int roundTo)
        {
            return (int)Math.Round(component / (double)roundTo) * roundTo;
        }

        public async Task<PrivateUser> GetCurrentUserProfile()
        {
            return await _spotify.UserProfile.Current();
        }

        public async Task<PlaylistDetailsViewModel> GetPlaylistDetails(string playlistId)
        {
            try
            {
                var playlist = await _spotify.Playlists.Get(playlistId);
                var tracks = new List<Track>();

                foreach (var item in playlist.Tracks.Items)
                {
                    if (item.Track is FullTrack track)
                    {
                        tracks.Add(new Track
                        {
                            Name = track.Name,
                            Artist = track.Artists.FirstOrDefault()?.Name,
                            Album = track.Album.Name,
                            Popularity = track.Popularity,
                            ImageUrl = track.Album.Images.FirstOrDefault()?.Url,
                            SongUrl = track.ExternalUrls["spotify"]
                        });
                    }
                }

                var playlistDetails = new PlaylistDetailsViewModel
                {
                    Name = playlist.Name,
                    ImageUrl = playlist.Images.FirstOrDefault()?.Url,
                    SpotifyUrl = playlist.ExternalUrls["spotify"],
                    Tracks = tracks
                };

                return playlistDetails;
            }
            catch (APIException ex)
            {
                Console.WriteLine($"Spotify API error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; 
            }
        }


    }


}
