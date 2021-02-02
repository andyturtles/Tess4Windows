using TessApi;

namespace Tess4Windows {

    public class Tess4WinSettings {

        public long? CarId;

        public double? latWindows;
        public double? lonWindows;

        public double? latHomelink;
        public double? lonHomelink;

        public bool HasHomelinkCoordinates {
            get { return ( latHomelink.HasValue && lonHomelink.HasValue ); }
        }

        public static Tess4WinSettings LoadSettings() {
            return TessTools.LoadResponse<Tess4WinSettings>(out _);
        }

        public void SaveSettings() {
            TessTools.SaveResponse(this);
        }
    }
}