require 'albacore'
require 'nuget_helper'
task :default => [:build]
dir = File.dirname(__FILE__)

desc "build solution"
build :build do |msb, args|
  msb.prop :configuration, :Debug
  msb.target = [:Rebuild]
  msb.sln = "redis-studies.sln"
end

desc "restore dependencies"
task :restore => [:restore_nuget, :restore_redis]

desc "Install missing NuGet packages."
nugets_restore :restore_nuget do |p|
  p.out = "packages"
  p.nuget_gem_exe
end

desc "build stack exchange redis"
build :restore_redis do |msb, args| 
  msb.prop :configuration, :Debug
  msb.target = [:Rebuild]
  msb.sln = File.join(dir, "StackExchange.Redis", "StackExchange.Redis","StackExchange.Redis.csproj")
end

desc "test using console"
test_runner :test => [:build] do |runner|
  runner.exe = NugetHelper::nunit_path
  files = Dir.glob(File.join(dir, "**", "bin", "Debug", "Tests.dll"))
  runner.files = files 
end

