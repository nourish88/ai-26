// lib/utils.ts

export function generateSlug(text: string): string {
  // A list of Turkish character mappings
  const turkishMap: { [key: string]: string } = {
    ç: "c",
    ğ: "g",
    ı: "i",
    ö: "o",
    ş: "s",
    ü: "u",
  };

  return text
    .toString()
    .toLowerCase()
    .replace(/[çğıöşü]/g, (char) => turkishMap[char]) // Convert Turkish chars
    .replace(/\s+/g, "-") // Replace spaces with -
    .replace(/[^\w-]+/g, "") // Remove all non-word chars
    .replace(/--+/g, "-") // Replace multiple - with single -
    .replace(/^-+/, "") // Trim - from start of text
    .replace(/-+$/, ""); // Trim - from end of text
}
