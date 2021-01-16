using TessApi;

namespace Tess4Windows {

    public class Tess4WinSettings {

        public long? CarId;

        public string lat = "50.000000";
        public string lon = "8.00000";

        public static Tess4WinSettings LoadSettings() {
            return TessTools.LoadResponse<Tess4WinSettings>(out _);
        }

        public void SaveSettings() {
            TessTools.SaveResponse(this);
        }
    }
}