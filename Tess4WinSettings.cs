using TessApi;

namespace Tess4Windows {

    public class Tess4WinSettings {

        public long? CarId;

        public string lat;
        public string lon;

        //public string test;

        public static Tess4WinSettings LoadSettings() {
            return TessTools.LoadResponse<Tess4WinSettings>(out _);
        }

        public void SaveSettings() {
            TessTools.SaveResponse(this);
        }
    }
}