"use client";
import { auth, signIn } from "@/auth";
import LoginBtn from "./ui/login-btn";
import SignOutBtn from "./ui/sign-out-button";
import { Session } from "next-auth";
import Image from "next/image";
export interface HeaderProps {
  session?: Session | null;
}

export default function Header({ session }: HeaderProps) {
  console.log("session", session);
  return (
    <header className="relative z-20 flex items-center justify-between p-6">
      {/* Logo */}

      <Image src="Negatif-Yazılı_Çalışma Yüzeyi 1.svg" alt="Logo" width={130} height={130} className="flex items-center" />
      {/* Navigation */}
      <nav className="flex items-center space-x-2">
        <a href="#" className="text-white/80 hover:text-white text-xs font-light px-3 py-2 rounded-full hover:bg-white/10 transition-all duration-200">
          Hakkında
        </a>
      </nav>
      <div className="flex gap-8">
        {" "}
        {session?.user ? (
          <div className="flex flex-row justify-between items-center gap-6">
            <div className="mr-4">{session.user.name}</div>
            <SignOutBtn name="signout" />
          </div>
        ) : (
          <LoginBtn />
        )}
      </div>
    </header>
  );
}
