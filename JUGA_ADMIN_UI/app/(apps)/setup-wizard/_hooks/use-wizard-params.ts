"use client";
import { useSearchParams, usePathname, useRouter } from "next/navigation";

export function useWizardParams() {
  const sp = useSearchParams();
  const pathname = usePathname();
  const router = useRouter();

  const step = sp.get("step") ?? "app";
  const applicationId = sp.get("applicationId");
  const appName = sp.get("appName");
  const identifier = sp.get("identifier");

  const replaceParams = (params: URLSearchParams) => {
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };

  const setParam = (k: string, v: string | number | null | undefined) => {
    const params = new URLSearchParams(sp.toString());
    if (v === null || v === undefined || v === "") params.delete(k);
    else params.set(k, String(v));
    replaceParams(params);
  };

  const setMany = (dict: Record<string, string | number | null | undefined>) => {
    const params = new URLSearchParams(sp.toString());
    Object.entries(dict).forEach(([k, v]) => {
      if (v === null || v === undefined || v === "") params.delete(k);
      else params.set(k, String(v));
    });
    replaceParams(params);
  };

  return {
    step,
    applicationId,
    appName,
    identifier,
    setParam,
    setMany,
    setStep: (s: string) => setParam("step", s),
    setApplicationId: (id: number | string) => setParam("applicationId", id),
    setAppName: (name: string) => setParam("appName", name),
    setIdentifier: (value: string) => setParam("identifier", value),
  };
}
