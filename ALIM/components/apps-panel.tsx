// components/apps-panel.tsx

"use client";

import { useRouter } from "next/navigation";
import { motion } from "framer-motion";
import { AppDefinition } from "@/app/apps/[identifier]/_types/message";

// --- Animation variants remain unchanged ---
const panelVariants = {
  hidden: { y: "100%", opacity: 0 },
  visible: { y: 0, opacity: 1 },
  exit: { y: "100%", opacity: 0 },
};
const cardVariants = {
  initial: { height: "auto", transformOrigin: "top" },
  hover: {
    scaleY: 1.05,
    transformOrigin: "top",
    transition: { duration: 0.3, ease: "easeOut" },
  },
};
const descriptionVariants = {
  initial: { opacity: 0, height: 0, marginTop: 0 },
  hover: {
    opacity: 1,
    height: "auto",
    marginTop: 12,
    transition: { duration: 0.3, ease: "easeOut" },
  },
};

// ✅ STEP 1: REMOVE the old `ajanData` constant. It will be replaced by the `apps` prop.

// ✅ STEP 2: KEEP the `appsData` constant. This section will remain static as requested.
const appsData = [
  {
    name: "Biyometrik Fotoğraf Analizi",
    description: "Biyometrik fotoğrafların ICAO kurallarına uygunluğu kontrol eden gelişmiş analiz aracı.",
    icon: "✈️",
    href: "/biyometrik-fotograf-analizi",
    disabled: false,
  },
  {
    name: "JanKod",
    description: "Jandarma personeli için özel olarak geliştirilmiş, kod yazımını hızlandıran yapay zeka asistanı.",
    icon: "💻",
    href: "#",
    disabled: true,
  },
];

// ✅ STEP 3: Update the component's props to receive the typed `apps` array.
export default function AppsPanel({ onClose, apps }: { onClose: () => void; apps: AppDefinition[] }) {
  const router = useRouter();

  return (
    <motion.div variants={panelVariants} initial="hidden" animate="visible" exit="exit" transition={{ duration: 0.5, ease: [0.22, 1, 0.36, 1] }} className="fixed bottom-0 left-0 z-50 w-full h-[85vh] backdrop-blur-md text-white p-6 overflow-y-auto rounded-t-2xl bg-black/30 border-t border-white/10">
      {/* Close Button (Unchanged) */}
      <button onClick={onClose} className="absolute top-6 right-6 p-2 rounded-full bg-white/10 hover:bg-white/20 transition-colors">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
          <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
        </svg>
        <span className="sr-only">Close</span>
      </button>

      {/* Main content grid (Unchanged) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 max-w-6xl mx-auto mt-8">
        {/* --- DYNAMIC AJANLAR SECTION --- */}
        <div>
          <h2 className="text-3xl font-bold mb-8">Yapay Zeka Ajanları ve Asistanları</h2>
          <div className="grid grid-cols-1 gap-4">
            {/* ✅ STEP 4: Map over the `apps` prop instead of `ajanData` */}
            {apps.length === 0 && "Lütfen Giriş Yapın"}
            {apps.map((app, index) => (
              <motion.div
                key={app.identifier} // Use unique identifier for key
                // ✅ Navigate to the dynamic app route
                onClick={() => router.push(`/apps/${app.identifier}`)}
                className="group cursor-pointer"
                variants={cardVariants as any}
                initial="initial"
                whileHover="hover"
                animate={{
                  opacity: 1,
                  y: 0,
                  transition: { delay: 0.3 + index * 0.1, duration: 0.5, ease: "easeOut" },
                }}
              >
                <div className="relative bg-white/5 backdrop-blur-sm border border-white/10 p-4 rounded-xl transition-all duration-300">
                  <div className="flex items-center gap-3 mb-2">
                    {/* Use a placeholder icon since it's not in the data */}

                    {/* Use data from the `app` object */}
                    <h3 className="text-lg font-semibold text-white">{app.name}</h3>
                  </div>
                  <motion.div variants={descriptionVariants as any} className="overflow-hidden">
                    <p className="text-gray-300 text-sm leading-relaxed">{app.description}</p>
                  </motion.div>
                  <div className="mt-3 text-blue-400 text-sm font-medium opacity-0 group-hover:opacity-100 transition-opacity">Uygulamayı Başlat →</div>
                </div>
              </motion.div>
            ))}
          </div>
        </div>

        {/* --- STATIC UYGULAMALAR SECTION (Unchanged) --- */}
        <div>
          <h2 className="text-3xl font-bold mb-8">Yapay Zeka Araçları</h2>
          <div className="grid grid-cols-1 gap-4">
            {appsData.map((app, index) => (
              <motion.div
                key={app.name}
                onClick={() => !app.disabled && router.push(app.href)}
                className={`group ${app.disabled ? "cursor-not-allowed opacity-60" : "cursor-pointer"}`}
                variants={cardVariants as any}
                initial="initial"
                whileHover="hover"
                animate={{
                  opacity: 1,
                  y: 0,
                  transition: { delay: 0.5 + index * 0.1, duration: 0.5, ease: "easeOut" },
                }}
              >
                <div className="relative bg-white/5 backdrop-blur-sm border border-white/10 p-4 rounded-xl transition-all duration-300 h-full">
                  {app.disabled && <div className="absolute top-3 right-3 bg-blue-500 text-white text-xs font-bold px-2 py-1 rounded-full">YAKINDA</div>}
                  <div className="flex items-center gap-3 mb-2">
                    <div className="text-2xl">{app.icon}</div>
                    <h3 className="text-lg font-semibold text-white">{app.name}</h3>
                  </div>
                  <motion.div variants={descriptionVariants as any} className="overflow-hidden">
                    <p className="text-gray-300 text-sm leading-relaxed">{app.description}</p>
                  </motion.div>
                  {!app.disabled && <div className="mt-3 text-blue-400 text-sm font-medium opacity-0 group-hover:opacity-100 transition-opacity">Uygulamayı Başlat →</div>}
                </div>
              </motion.div>
            ))}
          </div>
        </div>
      </div>
    </motion.div>
  );
}
