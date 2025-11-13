"use client";
import { signIn } from "@/app/api/auth/actions/signIn";
import React from "react";

function LoginBtn() {
  return (
    <form action={signIn}>
      {" "}
      <div id="gooey-btn" className="relative flex items-center group" style={{ filter: "url(#gooey-filter)" }}>
        <button className="px-6 py-2 rounded-full bg-white text-black font-normal text-xs transition-all duration-300 hover:bg-white/90 cursor-pointer h-8 flex items-center z-10">Giriş</button>
      </div>
    </form>
  );
}

export default LoginBtn;
