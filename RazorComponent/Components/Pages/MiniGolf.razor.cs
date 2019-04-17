using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace RazorComponent.Components.Pages
{
    public class MiniGolfModel : ComponentBase
    {
        public string PlayerNameToAdd { get; set; }
        public ObservableCollection<Player> Players { get; set; } = new ObservableCollection<Player>();
        public List<Player> RankedPlayers { get; set; } = new List<Player>();

        public int TrackParNumberToAdd { get; set; } = 3;
        public List<Track> Tracks { get; set; } = new List<Track>();

        public bool LastColumnToggleWorkaroundForDisplayIndex { get; set; } = true;

        public Track CurrentTrack { get; set; }

        public decimal DataGridHeight { get; set; }

        protected override Task OnInitAsync()
        {
            this.Players.CollectionChanged += this.Players_CollectionChanged;

            this.AddTrack();
            this.AddTrack();
            this.AddTrack();
            this.AddTrack();
            this.AddTrack();

            this.AddPlayer(null);
            this.AddPlayer(null);
            this.AddPlayer(null);

            this.RefreshDataGridHeight(this.Tracks.Count);

            return base.OnInitAsync();
        }

        private void Players_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RefreshPlayerRanking();
        }

        protected void AddTrack()
        {
            this.Tracks.Add(new Track() { Number = this.Tracks.Count + 1, Par = this.TrackParNumberToAdd });
        }

        protected void AddPlayer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Player {this.Players.Count + 1}";
            }

            var p = new Player() { Id = (this.Players.Count + 1), Name = name };
            foreach (var track in this.Tracks)
            { // bei allen bestehenden tracks den neuen player hinzufügen
                track.PlayerHits[p.Id] = null;
            }
            this.Players.Add(p);

            this.PlayerNameToAdd = null;
            this.RepositionEditColumn();
        }

        protected void RemovePlayer()
        {
            if (this.Players.Count > 0)
            {
                var p = this.Players.Last();
                foreach (var track in this.Tracks)
                {
                    track.PlayerHits.Remove(p.Id);
                }
                this.Players.Remove(p);

                this.RepositionEditColumn();
            }
        }

        protected void RepositionEditColumn()
        {
            this.LastColumnToggleWorkaroundForDisplayIndex = false;

            var _ = this.InvokeAsync(async () =>
            {
                await Task.Delay(1);
                this.LastColumnToggleWorkaroundForDisplayIndex = true;
                this.StateHasChanged();
            });
        }

        protected void AddTrack(int par)
        {
            var t = new Track() { Number = this.Tracks.Count + 1, Par = par };
            foreach (var player in this.Players)
            {
                t.PlayerHits[player.Id] = null;
            }

            this.Tracks.Add(t);

            this.RefreshDataGridHeight(this.Tracks.Count);
        }

        protected void RemoveTrack()
        {
            if (this.Tracks.Count > 0)
            {
                this.Tracks.RemoveAt(this.Tracks.Count - 1);
                this.RefreshDataGridHeight(this.Tracks.Count);
            }
        }

        private void RefreshDataGridHeight(int rowCount)
        {
            if (rowCount < 3)
            {
                rowCount = 3;
            }

            this.Invoke(() =>
            {
                this.DataGridHeight = (42M * rowCount) + 37M;
                this.StateHasChanged();
            });
        }

        protected void StartEdit(Track t)
        {
            if (t != null)
            {
                this.CurrentTrack = t;

                this.StateHasChanged();
            }
        }

        protected void EndEdit()
        {
            this.CurrentTrack = null;

            this.RefreshPlayerRanking();
        }

        private void RefreshPlayerRanking()
        {
            this.RankedPlayers = this.Players
                .OrderByDescending(a => this.Tracks.Count(b => b.PlayerHits[a.Id] != null)) // absteigend nach anzahl gespielter kurse
                .ThenBy(a => this.Tracks.Sum(b => b.PlayerHits[a.Id])) // aufsteigend nach summe der benötigten schläge
                .ToList();
        }

        protected void ResetGame()
        {
            this.Players.Clear();
            this.Tracks.Clear();
            this.PlayerNameToAdd = null;
            this.RepositionEditColumn();
            this.RefreshDataGridHeight(this.Tracks.Count);
        }
    }

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Track
    {
        public int Number { get; set; }
        public int Par { get; set; }

        public Dictionary<int, int?> PlayerHits { get; set; } = new Dictionary<int, int?>();
    }
}