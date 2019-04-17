using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor.Components.Grid;

namespace RazorComponent.Components.Pages
{
    public class MiniGolfModel : ComponentBase
    {
        public string PlayerNameToAdd { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();

        public int TrackParNumberToAdd { get; set; } = 4;
        public List<Track> Tracks { get; set; } = new List<Track>();

        public bool LastColumnToggleWorkaroundForDisplayIndex { get; set; } = true;

        public Track CurrentTrack { get; set; }

        public decimal DataGridHeight { get; set; }

        protected override Task OnInitAsync()
        {
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

        protected void AddTrack()
        {
            this.Tracks.Add(new Track() { Number = this.Tracks.Count + 1, Par = 4 });
        }

        protected void AddPlayer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Player {this.Players.Count + 1}";
            }

            var p = new Player() { Name = name };
            this.Players.Add(p);
            foreach (var track in this.Tracks)
            { // bei allen bestehenden tracks den neuen player hinzufügen
                track.PlayerHits[p] = null;
            }

            this.PlayerNameToAdd = null;
            this.RepositionEditColumn();
        }

        protected void RemovePlayer()
        {
            if (this.Players.Count > 0)
            {
                var p = this.Players.Last();
                this.Players.Remove(p);
                foreach (var track in this.Tracks)
                {
                    track.PlayerHits.Remove(p);
                }

                this.RepositionEditColumn();
            }
        }

        private void RepositionEditColumn()
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
                t.PlayerHits[player] = null;
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
            if (rowCount < 5)
                rowCount = 5;

            if (rowCount > 15)
                rowCount = 15;

            this.Invoke(() =>
            {
                this.DataGridHeight = (62M * rowCount) + 125M;
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
        }
    }

    public class Player
    {
        public string Name { get; set; }
    }

    public class Track
    {
        public int Number { get; set; }
        public int Par { get; set; }

        public Dictionary<Player, int?> PlayerHits { get; set; } = new Dictionary<Player, int?>();
    }
}